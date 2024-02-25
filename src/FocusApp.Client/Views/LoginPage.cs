using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Clients;
using FocusCore.Queries.User;
using Auth0.OidcClient;
using FocusApp.Client.Helpers;

namespace FocusApp.Client.Views;

internal class LoginPage : BasePage
{
    IAPIClient _client;
    private readonly Auth0Client auth0Client;
    IAuthenticationService _authenticationService;

    public LoginPage(IAPIClient client, Auth0Client authClient, IAuthenticationService authenticationService)
    {
        _client = client;
        auth0Client = authClient;
        _authenticationService = authenticationService;

        Content = new Grid
        {
            RowDefinitions = Rows.Define(60, 120, 80, 80, 80, 80, Star),
            ColumnDefinitions = Columns.Define(Star),
            BackgroundColor = AppStyles.Palette.LightPeriwinkle,

            Children =
            {
				// Skip Login Button
				new Button
                {
                    Text = "Skip",
                    TextColor = Colors.Black,
                    CornerRadius = 20,
                    BackgroundColor = AppStyles.Palette.Celeste
                }
                .Row(0)
                .Top()
                .Right()
                .Font(size: 15).Margins(top: 10, bottom: 10, left: 10, right: 10)
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    SkipButtonClicked(sender, eventArgs)),

				// Login Text
				new Label
                {
                    Text = "Login",
                    TextColor = Colors.Black,
                    FontSize = 40
                }
                .Row(2)
                .Center(),

				// Google sign in button
				new ImageButton
                {
                    Source = "g_sign_in"
                }
                .Row(3)
                .Center()
				// Logic when clicked
				.Invoke(button => button.Released += (sender, eventArgs) =>
                    OnLoginClicked(sender, eventArgs)),
            }
        };
    }

    private async void SkipButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///" + nameof(TimerPage));
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var loginResult = await auth0Client.LoginAsync();

        if (!loginResult.IsError)
        {
            _authenticationService.AuthToken = loginResult.AccessToken;
            Console.WriteLine("Login Page: " + _authenticationService.AuthToken);

            await Shell.Current.GoToAsync($"///" + nameof(TimerPage));
        }
        else
        {
            await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
        }
    }

    protected override async void OnAppearing()
    {
        var user = await _client.GetUser(new GetUserQuery { Id = Guid.NewGuid() });
        base.OnAppearing();
    }
}

