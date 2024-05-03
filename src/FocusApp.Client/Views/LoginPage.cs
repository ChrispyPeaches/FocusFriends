using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Helpers;
using Microsoft.Extensions.Logging;
using MediatR;
using FocusApp.Client.Methods.User;
using FocusApp.Shared.Models;
using CommunityToolkit.Mvvm.Input;

namespace FocusApp.Client.Views;

internal class LoginPage : BasePage
{
    IAuthenticationService _authenticationService;
    ILogger<LoginPage> _logger;
    IMediator _mediator;
    PopupService _popupService;

    public LoginPage(
        IAuthenticationService authenticationService,
        ILogger<LoginPage> logger,
        IMediator mediator,
        PopupService popupService
        )
    {
        _authenticationService = authenticationService;
        _logger = logger;
        _mediator = mediator;
        _popupService = popupService;

        var pets = new List<string> { "pet_beans.png", "pet_bob.png", "pet_danole.png", "pet_franklin.png", "pet_greg.png", "pet_wurmy.png" };
        var rnd = new Random();

        Loaded += LoginPage_Loaded;

        Content = new Grid
        {
            RowDefinitions = Rows.Define(200, 80, Star, 80, 80, GridRowsColumns.Stars(2)),
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
                .BindTapGesture(
                    commandSource: this,
                    commandPath: MiscHelper.NameOf(() => TapLoginSignupCommand)),

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
                .BindTapGesture(
                    commandSource: this,
                    commandPath: MiscHelper.NameOf(() => TapSkipCommand)),

                // Logo 
                new Image
                {
                    Source = "logo_with_border.png",
                    WidthRequest = 75,
                    HeightRequest = 75,
                }
                .Row(5)
                .Center()
            }
        };
    }

    /// <summary>
    /// Setup command to disallow concurrent execution
    /// </summary>
    public AsyncRelayCommand TapSkipCommand => new(OnTapSkipButton, AsyncRelayCommandOptions.None);

    private async Task OnTapSkipButton()
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

    /// <summary>
    /// Setup command to disallow concurrent execution
    /// </summary>
    public AsyncRelayCommand TapLoginSignupCommand => new(OnTapLoginSignup, AsyncRelayCommandOptions.None);

    private async Task OnTapLoginSignup()
    {
        try
        {

            // Handle login process on non-UI thread
            GetUserLogin.Result loginResult = await Task.Run(() => _mediator.Send(new GetUserLogin.Query()));

            if (loginResult.IsSuccessful)
            {
                _authenticationService.Auth0Id = loginResult.CurrentUser?.Auth0Id;
                _authenticationService.AuthToken = loginResult.AuthToken;
                _authenticationService.CurrentUser = loginResult.CurrentUser;

                _authenticationService.SelectedBadge = loginResult.CurrentUser?.SelectedBadge;
                _authenticationService.SelectedDecor = loginResult.CurrentUser?.SelectedDecor;
                _authenticationService.SelectedIsland = loginResult.CurrentUser?.SelectedIsland;
                _authenticationService.SelectedPet = loginResult.CurrentUser?.SelectedPet;
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

        await Shell.Current.GoToAsync($"///" + nameof(TimerPage));
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

            _authenticationService.SelectedIsland ??= result.Island;
            _authenticationService.SelectedPet ??= result.Pet;
            _authenticationService.SelectedDecor ??= result.Decor;
        }
    }

    protected override async void OnAppearing()
    {
        await AppShell.Current.SetTabBarIsVisible(false);
    }

    private async void LoginPage_Loaded(object sender, EventArgs e)
    {
        Task.Run(TryLoginFromStoredToken);
    }

    private async Task<GetPersistentUserLogin.Result> TryLoginFromStoredToken()
    {
        _popupService.ShowPopup<GenericLoadingPopupInterface>();
        try
        {
            var result = await _mediator.Send(new GetPersistentUserLogin.Query(), default);

            if (result.IsSuccessful)
            {
                await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync($"///" + nameof(TimerPage)));
            }
            else
            {
                _logger.LogInformation(result.Message);
            }

            _popupService.HidePopup();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User was not found in database, or the user had no identity token in secure storage. Please manually log in as the user.");
        }

        _popupService.HidePopup();
        return new GetPersistentUserLogin.Result
        {
            IsSuccessful = false
        };
    }
}

