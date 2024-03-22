using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Dtos;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using SimpleToolkit.SimpleShell.Extensions;
using Auth0.OidcClient;
using FocusApp.Client.Views.Mindfulness;
using FocusCore.Extensions;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Views;

internal class TimerPage : BasePage
{
    private ITimerService _timerService;
    private IDispatcherTimer? _timeStepperTimer;
    private readonly Auth0Client auth0Client;
    IAuthenticationService _authenticationService;
    private bool loggedIn;
    private string _selectedText;
    Button LogButton;
    Helpers.PopupService _popupService;
    private bool _showMindfulnessTipPopupOnStartSettingPlaceholder;
    private readonly ILogger<TimerPage> _logger;

    enum Row { TopBar, TimerDisplay, Island, PetAndIsland, MiddleWhiteSpace, TimerButtons, BottomWhiteSpace }
    enum Column { LeftTimerButton, TimerAmount, RightTimerButton }
    public enum TimerButton { Up, Down }

    public TimerPage(ITimerService timerService, Auth0Client authClient, IAuthenticationService authenticationService, Helpers.PopupService popupService, ILogger<TimerPage> logger)
    {
        _selectedText = "";
        _authenticationService = authenticationService;
        _timerService = timerService;
        auth0Client = authClient;
        _popupService = popupService;
        _logger = logger;

        _showMindfulnessTipPopupOnStartSettingPlaceholder = true;
        Appearing += ShowMindfulnessTipPopup;

        Island islandPlaceholder = new Island()
        {
            Name = "Default",
            ImagePath = "island_zero.png"
        };

        Pet petPlaceholder = new Pet()
        {
            Name = "Cat",
            ImagePath = "pet_cat_zero.png",
            HeightRequest = 90
        };

        // Login/Logout Button
        // This is placed here and not in the grid so the text
        //  can be dynamically updated
        LogButton = new Button
        {
            Text = _selectedText,
            BackgroundColor = AppStyles.Palette.Celeste,
            TextColor = Colors.Black,
            CornerRadius = 20
        }
        .Row(Row.TopBar)
        .Column(Column.RightTimerButton)
        .Top()
        .Right()
        .Font(size: 15).Margins(top: 10, bottom: 10, left: 10, right: 10)
        .Bind(IsVisibleProperty,
                        getter: (ITimerService th) => th.AreStepperButtonsVisible, source: _timerService)
        .Invoke(button => button.Released += (sender, eventArgs) =>
        {
        if (loggedIn)
        {
            OnLogoutClicked(sender, eventArgs);
        }
        else
        {
            OnLoginClicked(sender, eventArgs);
        };
        });

        Content = new Grid
        {
            RowDefinitions = GridRowsColumns.Rows.Define(
                (Row.TopBar, GridRowsColumns.Stars(1)),
                (Row.TimerDisplay, GridRowsColumns.Stars(1)),
                (Row.Island, GridRowsColumns.Stars(3)),
                (Row.PetAndIsland, GridRowsColumns.Stars(1)),
                (Row.MiddleWhiteSpace, GridRowsColumns.Stars(1)),
                (Row.TimerButtons, GridRowsColumns.Stars(1)),
                (Row.BottomWhiteSpace, GridRowsColumns.Stars(1))
                ),
            ColumnDefinitions = GridRowsColumns.Columns.Define(
                (Column.LeftTimerButton, GridRowsColumns.Stars(1)),
                (Column.TimerAmount, GridRowsColumns.Stars(2)),
                (Column.RightTimerButton, GridRowsColumns.Stars(1))
                ),
            BackgroundColor = AppStyles.Palette.LightPeriwinkle,
            Children =
            {
                // Setting Button
                new Button
                {     
                    Text = SolidIcons.Gears,
                    TextColor = Colors.Black,
                    FontFamily = nameof(SolidIcons),
                    FontSize = 40,
                    BackgroundColor = Colors.Transparent
                }
                .Row(Row.TopBar)
                .Top()
                .Left()
                .Bind(IsVisibleProperty,
                        getter: (ITimerService th) => th.AreStepperButtonsVisible, source: _timerService)
                .Invoke(button => button.Released += (sender, eventArgs) =>
                        SettingsButtonClicked(sender, eventArgs)),
                        

                // Time Left Display
                new Label
                {
                    BindingContext = _timerService,
                    FontSize = 70,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
                .Center()
                .Row(Row.TimerDisplay)
                .ColumnSpan(typeof(Column).GetEnumNames().Length)
                .Bind(Label.TextProperty,
                        getter: static (ITimerService th) => th.TimerDisplay),

                // Island
                new Image
                {
                    Source = islandPlaceholder.ImagePath,
                }
                .Row(Row.Island)
                .RowSpan(3)
                .ColumnSpan(typeof(Column).GetEnumNames().Length)
                .Margins(left: 10, right: 10),

                // Pet
                new Image
                {
                    Source = petPlaceholder.ImagePath,
                    MaximumHeightRequest = 200,
                    HeightRequest = petPlaceholder.HeightRequest
                }
                .Row(Row.PetAndIsland)
                .Column(Column.TimerAmount)
                .Margins(bottom: 60)
                .Bottom()
                .End(),

                // Increase Time Button
                new Button
                {
                    Text = SolidIcons.ChevronUp,
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Black,
                    FontSize = 30,
                }
                .Font(family: nameof(SolidIcons), size: 40)
                .End()
                .CenterVertical()
                .Row(Row.TimerButtons)
                .Column(Column.LeftTimerButton)
                .Bind(IsVisibleProperty,
                        getter: (ITimerService th) => th.AreStepperButtonsVisible, source: _timerService)
                .Invoke(button => button.Clicked += (sender, eventArgs) => 
                        onTimeStepperButtonClick(TimerButton.Up))
                .Invoke(button => button.Pressed += (sender, eventArgs) =>
                        onTimeStepperButtonPressed(TimerButton.Up))
                .Invoke(button => button.Released += (sender, eventArgs) =>
                        onTimeStepperButtonReleased()),

                // Toggle Timer Button
                new Button
                {
                    BindingContext = _timerService,
                    TextColor = Colors.Black,
                    CornerRadius = 20,
                }
                .Font(size: 20).Margins(left: 10, right: 10)
                .CenterVertical()
                .Row(Row.TimerButtons)
                .Column(Column.TimerAmount)
                .Bind(Button.TextProperty,
                        getter: static (ITimerService th) => th.ToggleTimerButtonText)
                .Bind(BackgroundColorProperty,
                        getter: static (ITimerService th) => th.ToggleTimerButtonBackgroudColor)
                .Invoke(button => button.Clicked += (sender, eventArgs) =>
                        _timerService.ToggleTimer.Invoke()),

                // Decrease Time Button
                new Button
                {
                    BindingContext = _timerService,
                    Text = SolidIcons.ChevronDown,
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Black
                }
                .Font(family: nameof(SolidIcons), size: 40)
                .Start()
                .CenterVertical()
                .Row(Row.TimerButtons)
                .Column(Column.RightTimerButton)
                .Bind(IsVisibleProperty, 
                        getter: (ITimerService th) => th.AreStepperButtonsVisible, source: _timerService )
                .Invoke(button => button.Clicked += (sender, eventArgs) =>
                        onTimeStepperButtonClick(TimerButton.Down))
                .Invoke(button => button.Pressed += (sender, eventArgs) =>
                        onTimeStepperButtonPressed(TimerButton.Down))
                .Invoke(button => button.Released += (sender, eventArgs) =>
                        onTimeStepperButtonReleased()),

                LogButton
            }
        };
    }

    /// <summary>
    /// Increment or decrement the timer duration.
    /// </summary>
    public void onTimeStepperButtonClick(TimerButton clickedButton)
    {
        int _stepRate = (int)TimeSpan.FromMinutes(1).TotalSeconds;

        _timerService.TimeLeft = clickedButton switch
        {
            TimerButton.Up => _timerService.TimeLeft + _stepRate,
            TimerButton.Down => (_timerService.TimeLeft > _stepRate) ?
                                                _timerService.TimeLeft - _stepRate
                                                : _stepRate,
            _ => 0
        };
    }

    /// <summary>
    /// Start the time duration stepper timer while the user holds the button.
    /// </summary>
    public void onTimeStepperButtonPressed(TimerButton clickedButton)
    {
        _timeStepperTimer = Application.Current!.Dispatcher.CreateTimer();
        _timeStepperTimer.Interval = TimeSpan.FromMilliseconds(200);
        _timeStepperTimer.Tick += (sender, e) => onTimeStepperButtonClick(clickedButton);
        _timeStepperTimer.Start();
    }

    /// <summary>
    /// Stop the time duration stepper timer.
    /// </summary>
    public void onTimeStepperButtonReleased()
    {
        if (_timeStepperTimer is not null)
        {
            _timeStepperTimer.Stop();
            _timeStepperTimer = null;
        }
    }

    private async void SettingsButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
            await Shell.Current.GoToAsync($"///{nameof(TimerPage)}/{nameof(SettingsPage)}");
        }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///" + nameof(LoginPage));
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var logoutResult = await auth0Client.LogoutAsync();
        _authenticationService.AuthToken = "";

        await Shell.Current.GoToAsync($"///" + nameof(LoginPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        loggedIn = !string.IsNullOrEmpty(_authenticationService.AuthToken);
        _selectedText = loggedIn ? "Logout" : "Login";
        LogButton.Text = _selectedText;
    }

    /// <summary>
    /// Remove subscription to the this.Appearing event so that the popup is only shown once on app startup,
    /// then show and populate the mindfulness tip popup.
    /// </summary>
    private async void ShowMindfulnessTipPopup(object? sender, EventArgs eventArgs)
    {
        try
        {
            await Task.Run(async () =>
            {
                Appearing -= ShowMindfulnessTipPopup;
                Thread.Sleep(1000);
                MindfulnessTipPopupInterface tipPopup =
                    (MindfulnessTipPopupInterface)_popupService.ShowAndGetPopup<MindfulnessTipPopupInterface>();
                await tipPopup?.PopulatePopup(MindfulnessTipExtensions.FocusSessionRating.Good, default)!;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured when showing and populating startup mindfulness tip.");
        }
        
    }
}