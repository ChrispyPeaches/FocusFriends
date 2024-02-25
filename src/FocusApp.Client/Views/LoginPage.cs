using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Clients;
using FocusCore.Queries.User;
using FocusApp.Client.Models;
using FocusApp.Client.Views;
using Auth0.OidcClient;

namespace FocusApp.Client.Views;

internal class LoginPage : BasePage
{
    IAPIClient _client;
    private readonly Auth0Client auth0Client;

    public LoginPage(IAPIClient client, Auth0Client authClient)
    {
        _client = client;
        auth0Client = authClient;

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
            var _user = loginResult.User;
            var name = _user.FindFirst(c => c.Type == "name")?.Value;
            var email = _user.FindFirst(c => c.Type == "email")?.Value;

            Console.WriteLine(name, email);

            await Shell.Current.GoToAsync("///" + nameof(TimerPage));
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

