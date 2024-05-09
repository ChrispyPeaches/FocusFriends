using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Helpers;
using Microsoft.Extensions.Logging;
using MediatR;
using FocusApp.Client.Methods.User;
using FocusApp.Shared.Models;
using CommunityToolkit.Mvvm.Input;
using IdentityModel.OidcClient;

namespace FocusApp.Client.Views;

internal class LoginPage : BasePage
{
    IAuthenticationService _authenticationService;
    ILogger<LoginPage> _logger;
    IMediator _mediator;
    PopupService _popupService;
    private readonly ISyncService _syncService;

    public LoginPage(
        IAuthenticationService authenticationService,
        ILogger<LoginPage> logger,
        IMediator mediator,
        ISyncService syncService,
        PopupService popupService
        )
    {
        _authenticationService = authenticationService;
        _logger = logger;
        _mediator = mediator;
        _popupService = popupService;
        _syncService = syncService;

        var pets = new List<string> { "pet_cool_cat.png", "pet_cool_cat.png", "pet_cool_cat.png", "pet_cooler_cat.png",  };
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

            if (loginResult.IsSuccessful && loginResult.CurrentUser is not null)
            {
                _authenticationService.PopulateWithUserData(loginResult.CurrentUser);
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

    /// <summary>
    /// Failsafe: Initialize a user with empty data
    /// </summary>
    private async Task InitializeEmptyUser()
    {
        // If a user isn't logged in, clear the user data
        if (_authenticationService.Auth0Id == null)
        {
            _authenticationService.ClearUser();
        }

        // Add the initial/default island and pet if they don't exist
        if (_authenticationService.SelectedIsland is null || _authenticationService.SelectedPet is null)
        {
            GetDefaultItems.Result result = await _mediator.Send(new GetDefaultItems.Query());

            _authenticationService.SelectedIsland ??= result.Island;
            _authenticationService.SelectedPet ??= result.Pet;
        }
    }

    protected override async void OnAppearing()
    {
        await AppShell.Current.SetTabBarIsVisible(false);
    }

    /// <summary>
    /// Run startup logic on app startup, but not after
    /// </summary>
    private async void LoginPage_Loaded(object sender, EventArgs e)
    {
        Loaded -= LoginPage_Loaded;

        _ = Task.Run(async () =>
        {
            // Only show the content downloading popup for getting the essential data
            await _popupService.ShowPopupAsync<SyncDataLoadingPopupInterface>();
            try
            {
                await _syncService.GatherEssentialDatabaseData();
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "Error",
                    "There was an issue downloading content. Please ensure you have a stable internet connection and restart the app.",
                    "OK");
            }

            await _popupService.HidePopupAsync<SyncDataLoadingPopupInterface>();

            await TryLoginFromStoredToken();
            _authenticationService.StartupSyncTask = _syncService.StartupSync();
        });
    }

    private async Task TryLoginFromStoredToken()
    {
        await _popupService.ShowPopupAsync<GenericLoadingPopupInterface>();
        try
        {
            var result = await _mediator.Send(new GetPersistentUserLogin.Query());
            
            if (result.IsSuccessful)
            {
                await _popupService.HidePopupAsync<GenericLoadingPopupInterface>();
                await MainThread.InvokeOnMainThreadAsync(async () => await Shell.Current.GoToAsync($"///" + nameof(TimerPage)));
                return;
            }
            await _popupService.HidePopupAsync<GenericLoadingPopupInterface>();

            _logger.LogInformation(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User was not found in database, or the user had no identity token in secure storage. Please manually log in as the user.");
        }
    }
}

