﻿using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using SimpleToolkit.SimpleShell.Extensions;
using Auth0.OidcClient;
using CommunityToolkit.Maui.Converters;
using FocusApp.Client.Views.Controls;
using FocusApp.Client.Views.Mindfulness;
using FocusApp.Shared.Models;
using FocusCore.Extensions;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Views;

internal class TimerPage : BasePage
{
    private readonly ITimerService _timerService;
    private IDispatcherTimer? _timeStepperTimer;
    private readonly Auth0Client _auth0Client;
    private readonly IAuthenticationService _authenticationService;
    private bool _loggedIn;
    private string _selectedText;
    private readonly Button _logButton;
    private readonly Helpers.PopupService _popupService;
    private bool _showMindfulnessTipPopupOnStartSettingPlaceholder;
    private readonly ILogger<TimerPage> _logger;

    enum Row { TopBar, TimerDisplay, IslandView, TimerButtons, TabBarSpacer }
    enum Column { LeftTimerButton, TimerAmount, RightTimerButton }
    private enum TimerButton { Up, Down }

    public TimerPage(
        ITimerService timerService,
        Auth0Client authClient,
        IAuthenticationService authenticationService,
        PopupService popupService,
        ILogger<TimerPage> logger)
    {
        _selectedText = "";
        _authenticationService = authenticationService;
        _timerService = timerService;
        _auth0Client = authClient;
        _popupService = popupService;
        _logger = logger;

        if (PreferencesHelper.Get<bool>(PreferencesHelper.PreferenceNames.startup_tips_enabled))
            Appearing += ShowMindfulnessTipPopup;

        // Login/Logout Button
        // This is placed here and not in the grid so the text
        //  can be dynamically updated
        _logButton = new Button
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
        .Bind(
            IsVisibleProperty,
            getter: (ITimerService th) => th.AreStepperButtonsVisible,
            source: _timerService)
        .Invoke(button => button.Released += (sender, eventArgs) =>
        {
        if (_loggedIn)
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
                (Row.IslandView, GridRowsColumns.Stars(4)),
                (Row.TimerButtons, GridRowsColumns.Stars(1)),
                (Row.TabBarSpacer, Consts.TabBarHeight)
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
                .Invoke(button => button.Released += SettingsButtonClicked),
                        

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
                SetupIslandDisplayView(),

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
                        OnTimeStepperButtonClick(TimerButton.Up))
                .Invoke(button => button.Pressed += (sender, eventArgs) =>
                        OnTimeStepperButtonPressed(TimerButton.Up))
                .Invoke(button => button.Released += (sender, eventArgs) =>
                        OnTimeStepperButtonReleased()),

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
                        OnTimeStepperButtonClick(TimerButton.Down))
                .Invoke(button => button.Pressed += (sender, eventArgs) =>
                        OnTimeStepperButtonPressed(TimerButton.Down))
                .Invoke(button => button.Released += (sender, eventArgs) =>
                        OnTimeStepperButtonReleased()),

                _logButton
            }
        };
    }

    private IslandDisplayView SetupIslandDisplayView()
    {
        return new IslandDisplayView(parentPage: this)
            {
                BindingContext = _authenticationService,
            }
            .Center()
            .FillHorizontal()
            .Row(Row.IslandView)
            .ColumnSpan(typeof(Column).GetEnumNames().Length)
            .Bind(
                IslandDisplayView.IslandProperty,
                getter: static (IAuthenticationService authService) => authService.SelectedIsland,
                source: _authenticationService)
            .Bind(
                IslandDisplayView.PetProperty,
                getter: static (IAuthenticationService authService) => authService.SelectedPet,
                source: _authenticationService)
            .Bind(
                IslandDisplayView.DisplayDecorProperty,
                getter: static (IAuthenticationService authService) => authService.SelectedDecor,
                source: _authenticationService);
    }

    /// <summary>
    /// Increment or decrement the timer duration.
    /// </summary>
    private void OnTimeStepperButtonClick(TimerButton clickedButton)
    {
        int stepRate = (int)TimeSpan.FromMinutes(1).TotalSeconds;

        _timerService.TimeLeft = clickedButton switch
        {
            TimerButton.Up => _timerService.TimeLeft + stepRate,
            TimerButton.Down => (_timerService.TimeLeft > stepRate) ?
                                                _timerService.TimeLeft - stepRate
                                                : stepRate,
            _ => 0
        };
    }

    /// <summary>
    /// Start the time duration stepper timer while the user holds the button.
    /// </summary>
    private void OnTimeStepperButtonPressed(TimerButton clickedButton)
    {
        _timeStepperTimer = Application.Current!.Dispatcher.CreateTimer();
        _timeStepperTimer.Interval = TimeSpan.FromMilliseconds(200);
        _timeStepperTimer.Tick += (sender, e) => OnTimeStepperButtonClick(clickedButton);
        _timeStepperTimer.Start();
    }

    /// <summary>
    /// Stop the time duration stepper timer.
    /// </summary>
    private void OnTimeStepperButtonReleased()
    {
        if (_timeStepperTimer is not null)
        {
            _timeStepperTimer.Stop();
            _timeStepperTimer = null;
        }
    }

    private async void SettingsButtonClicked(object? sender, EventArgs e)
    {
        Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///" + nameof(LoginPage));
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var logoutResult = await _auth0Client.LogoutAsync();
        _authenticationService.AuthToken = "";

        await Shell.Current.GoToAsync($"///" + nameof(LoginPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _loggedIn = !string.IsNullOrEmpty(_authenticationService.AuthToken);
        _selectedText = _loggedIn ? "Logout" : "Login";
        _logButton.Text = _selectedText;
    }

    /// <summary>
    /// Remove subscription to the this. Appearing event so that the popup is only shown once on app startup,
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