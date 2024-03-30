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
using FocusApp.Shared.Models;

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
        // If user skips login, initialize empty user and set selected pet and island to defaults
        try
        {
            await Task.Run(InitializeEmptyUser);
            _authenticationService.AuthToken = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing empty user.");
        }


        // Todo: If user skips login, set selected pet and island to defaults
        await Shell.Current.GoToAsync("///" + nameof(TimerPage));
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            // Handle login process on non-UI thread
            GetUserLogin.Result loginResult = await Task.Run(() => _mediator.Send(new GetUserLogin.Query()));

            // Todo: If user exists, sync user data with server including islands, pets, etc.

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
            _logger.LogError(ex, "Error during login process.");
        }

        // If any issues occured while logging in, initialize empty user and set selected pet and island to defaults
        try
        {
            await Task.Run(InitializeEmptyUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing empty user.");
        }

    }

    private async Task InitializeEmptyUser()
    {
        _authenticationService.CurrentUser ??= new User()
        {
            Auth0Id = "",
            Email = "",
            UserName = "",
            Balance = 0
        };

        if (_authenticationService.SelectedIsland is null || _authenticationService.SelectedPet is null)
        {
            GetDefaultItems.Result result = await _mediator.Send(new GetDefaultItems.Query());

            _authenticationService.CurrentUser.SelectedIsland ??= result.Island;
            _authenticationService.CurrentUser.SelectedPet ??= result.Pet;
            _authenticationService.CurrentUser.SelectedFurniture ??= result.Decor;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }
}

