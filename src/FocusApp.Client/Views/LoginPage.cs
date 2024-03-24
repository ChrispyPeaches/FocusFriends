using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Clients;
using Auth0.OidcClient;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using Microsoft.Extensions.Logging;
using MediatR;
using FocusApp.Client.Methods.User;

namespace FocusApp.Client.Views;

internal class LoginPage : BasePage
{
    IAuthenticationService _authenticationService;
    ILogger<LoginPage> _logger;
    IMediator _mediator;

    public LoginPage(IAuthenticationService authenticationService, ILogger<LoginPage> logger, IMediator mediator)
    {
        _authenticationService = authenticationService;
        _logger = logger;
        _mediator = mediator;

        var pets = new List<string> { "pet_beans.png", "pet_bob.png", "pet_danole.png", "pet_franklin.png", "pet_greg.png", "pet_wurmy.png" };
        var rnd = new Random();

        Content = new Grid
        {
            RowDefinitions = Rows.Define(200, 100, 100, 80, 80, Star, 50),
            ColumnDefinitions = Columns.Define(Star),
            BackgroundColor = AppStyles.Palette.LightPeriwinkle,

            Children =
            {
                // Pet Image
                new Image
                {
                    Source = pets[rnd.Next(0, pets.Count)],
                    WidthRequest = 150,
                    HeightRequest = 150
                }
                .Row(0)
                .Center()
                .Bottom(),

                // Label
                new Label
                {
                    Text = "Focus Friends",
                    TextColor = Colors.Black,
                }
                .Row(1)
                .Center()
                .Font(size: 40).Margins(top: 10, bottom: 10, left: 10, right: 10),

                // Login/Signup Button
				new Button
                {
                    Text = "Login/Signup",
                    TextColor = Colors.Black,
                    CornerRadius = 20,
                    BackgroundColor = AppStyles.Palette.Celeste
                }
                .Row(3)
                .CenterHorizontal()
                .Font(size: 25).Margins(top: 10, bottom: 10, left: 10, right: 10)
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    OnLoginClicked(sender, eventArgs)),

				// Skip Login Button
				new Button
                {
                    Text = "Continue Without Login",
                    TextColor = Colors.Black,
                    CornerRadius = 20,
                    BackgroundColor = AppStyles.Palette.Celeste
                }
                .Row(4)
                .CenterHorizontal()
                .Font(size: 25).Margins(top: 10, bottom: 10, left: 10, right: 10)
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    SkipButtonClicked(sender, eventArgs)),

                // Logo 
                new Image
                {
                    Source = "logo.png",
                    WidthRequest = 75,
                    HeightRequest = 75,
                }
                .Row(5)
                .Center()
            }
        };
    }

    private async void SkipButtonClicked(object sender, EventArgs e)
    {
        // If user skips login, set current user / auth token to null
        _authenticationService.CurrentUser = null;
        _authenticationService.AuthToken = null;
        await Shell.Current.GoToAsync("///" + nameof(TimerPage));
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            // Handle login process
            GetUserLogin.Result loginResult = await _mediator.Send(new GetUserLogin.Query());

            if (loginResult.IsSuccessful)
            {
                _authenticationService.AuthToken = loginResult.AuthToken;
                _authenticationService.CurrentUser = loginResult.CurrentUser;
                await Shell.Current.GoToAsync($"///" + nameof(TimerPage));
            }
            else
            {
                await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
            }
        }
        catch (Exception ex) 
        {
            _logger.Log(LogLevel.Error, "Error during login process. Exception: " + ex.Message);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }
}

