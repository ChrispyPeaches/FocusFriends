﻿using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Resources;
using FocusApp.Client.Views;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using FocusApp.Client.Clients;
using FocusApp.Client.Methods.User;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FocusApp.Client.Views.Mindfulness;
using FocusCore.Commands.User;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace FocusApp.Client.Helpers;

internal interface ITimerService
{
    TimerService.TimerDto TimerDisplay { get; set; }
    string ToggleTimerButtonText { get; }
    Color ToggleTimerButtonBackgroudColor { get; }
    bool AreStepperButtonsVisible { get; }
    int TimeLeft { get; set; }
    Action ToggleTimer { get; set; }
    event PropertyChangedEventHandler? PropertyChanged;
}

internal class TimerService : ITimerService, INotifyPropertyChanged
{
    public class TimerDto
    {
        [DisplayFormat(DataFormatString = "{0:00}")]
        public int HourTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:00}")]
        public int MinuteTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:00}")]
        public int SecondTime { get; set; }

        public override string ToString()
        {
            string hour = (HourTime != 0) ? $"{string.Format("{0:00}", HourTime)}:" : "";
            string minute = string.Format("{0:00}", MinuteTime);
            string second = string.Format("{0:00}", SecondTime);

            return $"{hour}{minute}:{second}";
        }
    }

    public enum TimerState
    {
        StoppedPreFocus,
        FocusCountdown,
        StoppedPreBreak,
        BreakCountdown
    }

    #region Fields

    private TimerDto _timerDisplay;
    private string _toggleTimerButtonText;
    private Color _toggleTimerButtonBackgroudColor;
    private bool _areStepperButtonsVisible;
    private int _timeLeft;
    private IDispatcherTimer? _timer;
    private DateTimeOffset? _lastKnownTime;
    private TimerState _state;
    private int _lastFocusTimerDuration;
    private int _lastBreakTimerDuration;
    private DateTimeOffset? _currentSessionStartTime;

    private Helpers.PopupService _popupService;
    private readonly IMediator _mediator;
    private readonly IBadgeService _badgeService;
    private readonly ILogger<TimerService> _logger;
    #endregion

    #region Properties

    public TimerDto TimerDisplay
    {
        get => _timerDisplay;
        set => SetProperty(ref _timerDisplay, value);
    }

    public string ToggleTimerButtonText
    {
        get => _toggleTimerButtonText;
        private set => SetProperty(ref _toggleTimerButtonText, value);
    }

    public Color ToggleTimerButtonBackgroudColor
    {
        get => _toggleTimerButtonBackgroudColor;
        private set => SetProperty(ref _toggleTimerButtonBackgroudColor, value);
    }

    public bool AreStepperButtonsVisible
    {
        get => _areStepperButtonsVisible;
        private set => SetProperty(ref _areStepperButtonsVisible, value);
    }

    public int TimeLeft
    {
        get => _timeLeft;
        set
        {
            int maxTime = (int)TimeSpan.FromHours(5).TotalSeconds;

            value = (value < 0) ? 0 : value;
            value = (value > maxTime) ? maxTime : value;

            SetProperty(ref _timeLeft, value);
            UpdateTimerDisplay();
        }
    }

    #endregion

    #region Delegates

    public event PropertyChangedEventHandler? PropertyChanged;
    public Action ToggleTimer { get; set; }

    #endregion

    private void UpdateTimerDisplay()
    {
        TimerDisplay = new TimerDto
        {
            HourTime = (int)Math.Floor(TimeLeft / 3600f),
            MinuteTime = ((int)Math.Floor(TimeLeft / 60f)) % 60,
            SecondTime = TimeLeft % 60
        };
    }


    public TimerService(
        Helpers.PopupService popupService,
        IMediator mediator,
        IBadgeService badgeService,
        ILogger<TimerService> logger)
    {
        _popupService = popupService;
        _mediator = mediator;
        _badgeService = badgeService;
        _logger = logger;

        _timerDisplay = new TimerDto();
        _lastFocusTimerDuration = (int)TimeSpan.FromMinutes(5).TotalSeconds;
        _lastBreakTimerDuration = (int)TimeSpan.FromMinutes(5).TotalSeconds;
        _state = TimerState.StoppedPreFocus;
        _toggleTimerButtonText = "Start Focus";
        _toggleTimerButtonBackgroudColor = AppStyles.Palette.Celeste;

        TimeLeft = _lastFocusTimerDuration;
        AreStepperButtonsVisible = true;
        ToggleTimer += TransitionToNextState;
    }

    private bool isTimerActive()
    {
        return (_state == TimerState.FocusCountdown ||
                _state == TimerState.BreakCountdown);
    }

    /// <summary>
    /// Move between states of the timer and perform transition logic
    /// </summary>
    private void TransitionToNextState()
    {
        _state = _state switch
        {
            TimerState.StoppedPreFocus => TimerState.FocusCountdown,
            TimerState.FocusCountdown => TimeLeft > 0 ?
                                            TimerState.StoppedPreFocus
                                            : TimerState.StoppedPreBreak,
            TimerState.StoppedPreBreak => TimerState.BreakCountdown,
            TimerState.BreakCountdown => TimerState.StoppedPreFocus,
            _ => TimerState.StoppedPreFocus
        };

        switch (_state)
        {
            case TimerState.StoppedPreFocus:
                onTimerStop();
                ToggleTimerButtonText = "Start Focus";
                ToggleTimerButtonBackgroudColor = AppStyles.Palette.Celeste;
                AreStepperButtonsVisible = true;
                break;

            case TimerState.FocusCountdown:
                onTimerStart();
                ToggleTimerButtonText = "Stop";
                ToggleTimerButtonBackgroudColor = AppStyles.Palette.OrchidPink;
                AreStepperButtonsVisible = false;
                _currentSessionStartTime = DateTimeOffset.UtcNow;
                break;

            case TimerState.StoppedPreBreak:
                onTimerStop();
                Task.Run(TrackFocusSession);
                ToggleTimerButtonText = "Start Break";
                ToggleTimerButtonBackgroudColor = AppStyles.Palette.Celeste;
                AreStepperButtonsVisible = true;
                ShowSessionRatingPopup();
                break;

            case TimerState.BreakCountdown:
                onTimerStart();
                ToggleTimerButtonText = "Skip";
                ToggleTimerButtonBackgroudColor = AppStyles.Palette.OrchidPink;
                AreStepperButtonsVisible = false;
                break;
        }
    }

    /// <summary>
    /// Log the focus session in the mobile database and api.
    /// </summary>
    private async Task TrackFocusSession()
    {
        if (!_currentSessionStartTime.HasValue)
        {
            return;
        }

        DateTimeOffset? endTime = (_currentSessionStartTime + TimeSpan.FromSeconds(_lastFocusTimerDuration)).Value.ToUniversalTime();

        try
        {

            await _mediator.Send(new AddSessionToUser.Query()
                {
                    SessionStartTime = _currentSessionStartTime.Value.ToUniversalTime(),
                    SessionEndTime = endTime.Value.ToUniversalTime()
                },
                default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while tracking focus session.");
        }

        _currentSessionStartTime = null;

        try
        {
            BadgeEligibilityResult result = await _badgeService.CheckFocusSessionBadgeEligibility();

            if (result is { IsEligible: true, EarnedBadge: not null })
            {
                _popupService.TriggerBadgeEvent<EarnedBadgePopupInterface>(result.EarnedBadge);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking for badge eligibility.");
        }
    }

    private void ShowSessionRatingPopup()
    {
        if (PreferencesHelper.Get<bool>(PreferencesHelper.PreferenceNames.session_rating_enabled))
            _popupService.ShowPopup<SessionRatingPopupInterface>();
    }

    /// <summary>
    /// Subscribe to app lifecycle events, save the timer duration, then setup and start a timer
    /// </summary>
    private void onTimerStart()
    {
        App.WindowDeactivated += onAppMinimized;
        App.WindowStopped += onAppMinimized;
        App.WindowDestroying += onAppMinimized;

        TimeLeft =
            (TimerDisplay.HourTime * 3600) +
            (TimerDisplay.MinuteTime * 60) +
             TimerDisplay.SecondTime;

        _ = _state switch
        {
            TimerState.FocusCountdown => _lastFocusTimerDuration = TimeLeft,
            TimerState.BreakCountdown => _lastBreakTimerDuration = TimeLeft,
            _ => 0
        };

        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (sender, eventArgs) =>
        {
            TimeLeft--;
            if (TimeLeft <= 0)
            {
                if (_state == TimerState.BreakCountdown)
                    Task.Run(CheckUserSessionBadge);

                TransitionToNextState();
            }
        };
        _timer.Start();
    }

    private async Task CheckUserSessionBadge()
    {
        try
        {
            BadgeEligibilityResult result = await _badgeService.CheckBreakSessionBadgeEligibility();

            if (result is { IsEligible: true, EarnedBadge: not null })
            {
                _popupService.TriggerBadgeEvent<EarnedBadgePopupInterface>(result.EarnedBadge);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking for badge eligibility.");
        }
    }

    /// <summary>
    /// Unsubscribe from app lifecyce events,
    /// stop the timer,
    /// and reset the timer to the last used duration for this state.
    /// </summary>
    private void onTimerStop()
    {
        App.WindowDeactivated -= onAppMinimized;
        App.WindowStopped -= onAppMinimized;
        App.WindowDestroying -= onAppMinimized;

        if (_timer != null)
        {
            _timer.Stop();
        }

        TimeLeft = _state switch
        {
            TimerState.StoppedPreFocus => _lastFocusTimerDuration,
            TimerState.StoppedPreBreak => _lastBreakTimerDuration,
            _ => 60,
        };

    }

    #region App Lifecycle Triggered Logic

    /// <summary>
    /// Rotate the subscribed app lifecycle events,
    /// stop the timer,
    /// and keep track of when the app was minimized.
    /// </summary>
    private void onAppMinimized()
    {
        App.WindowDeactivated -= onAppMinimized;
        App.WindowStopped -= onAppMinimized;

        App.WindowActivated += onAppResumed;
        App.WindowResumed += onAppResumed;

        if (_timer != null)
        {
            _timer.Stop();
        }
        _lastKnownTime = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// Rotate the app lifecycle events,
    /// subtract the time outside of the app from the time left on the timer,
    /// and continute the timer or transition to the next state
    /// </summary>
    private void onAppResumed()
    {
        App.WindowResumed -= onAppResumed;
        App.WindowActivated -= onAppResumed;

        App.WindowDeactivated += onAppMinimized;
        App.WindowStopped += onAppMinimized;
        
        if (_currentSessionStartTime != null)
        {
            TimeSpan? timeLaspedWhileMinimized = DateTimeOffset.UtcNow - _lastKnownTime;
            if (timeLaspedWhileMinimized.HasValue)
            {
                TimeLeft -= (int)timeLaspedWhileMinimized.Value.TotalSeconds;
            }
            else
            {
                TimeLeft = _lastFocusTimerDuration - (int)DateTimeOffset.UtcNow.Subtract(_currentSessionStartTime.Value).TotalSeconds;
            }

            if (TimeLeft <= 0) 
            {
                TransitionToNextState();
            }
            else if (_timer != null)
            {
                _timer.Start();
            }
        }
    }

    #endregion

    #region Property Changed Notification Logic

    private void SetProperty<T>(ref T backingStore, in T value, [CallerMemberName] in string propertyname = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return;
        }

        backingStore = value;

        OnPropertyChanged(propertyname);
    }

    void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    #endregion
}
