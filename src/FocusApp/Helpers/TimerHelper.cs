using CommunityToolkit.Maui.Markup;
using FocusApp.Resources;
using FocusApp.Views;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FocusApp.Helpers;

internal class TimerHelper : INotifyPropertyChanged
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
        StoppedPreStudy,
        StudyCountdown,
        StoppedPreBreak,
        BreakCountdown
    }

    #region Fields

    private TimerDto _timerDisplay;
    private string _toggleTimerButtonText;
    private int _timeLeft;
    private IDispatcherTimer? _timer;
    private DateTime? _lastKnownTime;
    private TimerState _state;
    private int _lastStudyTimerDuration;
    private int _lastBreakTimerDuration;

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

    private Color _toggleTimerButtonBackgroudColor;
    public Color ToggleTimerButtonBackgroudColor
    {
        get => _toggleTimerButtonBackgroudColor;
        private set => SetProperty(ref _toggleTimerButtonBackgroudColor, value);
    }

    private int TimeLeft
    {
        get => _timeLeft;
        set
        {
            value = (value < 0) ? 0 : value;

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

    public TimerHelper()
    {
        _timerDisplay = new TimerDto();
        _lastStudyTimerDuration = 60;
        _lastBreakTimerDuration = 60;
        _state = TimerState.StoppedPreStudy;
        _toggleTimerButtonText = "Start Studying";
        _toggleTimerButtonBackgroudColor = AppStyles.Palette.Celeste;
        TimeLeft = 60;
        ToggleTimer += TransitionToNextState;
    }

    public bool isTimerActive()
    {
        return (_state == TimerState.StudyCountdown ||
                _state == TimerState.BreakCountdown);
    }

    public void onTimeStepperButtonClick(TimerView.TimerButton clickedButton)
    {
        int stepAmount = 60;

        TimeLeft = clickedButton switch
        {
            TimerView.TimerButton.Up => TimeLeft + stepAmount,
            TimerView.TimerButton.Down => TimeLeft - stepAmount,
            _ => 0
        };
    }

    private void TransitionToNextState()
    {
        _state = _state switch
        {
            TimerState.StoppedPreStudy => TimerState.StudyCountdown,
            TimerState.StudyCountdown => TimeLeft > 0 ?
                                            TimerState.StoppedPreStudy
                                            : TimerState.StoppedPreBreak,
            TimerState.StoppedPreBreak => TimerState.BreakCountdown,
            TimerState.BreakCountdown => TimerState.StoppedPreStudy,
            _ => TimerState.StoppedPreStudy
        };

        switch (_state)
        {
            case TimerState.StoppedPreStudy:
                onTimerStop();
                ToggleTimerButtonText = "Start Studying";
                ToggleTimerButtonBackgroudColor = AppStyles.Palette.Celeste;
                break;

            case TimerState.StudyCountdown:
                onTimerStart();
                ToggleTimerButtonText = "Stop";
                ToggleTimerButtonBackgroudColor = AppStyles.Palette.OrchidPink;
                break;

            case TimerState.StoppedPreBreak:
                onTimerStop();
                ToggleTimerButtonText = "Start Break";
                ToggleTimerButtonBackgroudColor = AppStyles.Palette.Celeste;
                break;

            case TimerState.BreakCountdown:
                onTimerStart();
                ToggleTimerButtonText = "Skip";
                ToggleTimerButtonBackgroudColor = AppStyles.Palette.OrchidPink;
                break;
        }
    }

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
            TimerState.StudyCountdown => _lastStudyTimerDuration = TimeLeft,
            TimerState.BreakCountdown => _lastBreakTimerDuration = TimeLeft,
            _ => 0
        };

        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(1000);
        _timer.Tick += (sender, eventArgs) =>
        {
            TimeLeft--;
            if (TimeLeft <= 0)
            {
                TransitionToNextState();
            }
        };
        _timer.Start();
    }


    private void onTimerStop()
    {
        App.WindowDeactivated -= onAppMinimized;
        App.WindowStopped -= onAppMinimized;
        App.WindowDestroying -= onAppMinimized;

        TimeLeft = _state switch
        {
            TimerState.StoppedPreStudy => _lastStudyTimerDuration,
            TimerState.StoppedPreBreak => _lastBreakTimerDuration,
            _ => 60,
        };

        if (_timer != null)
        {
            _timer.Stop();
        }
    }

    #region App Lifecycle Triggered Logic

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
        _lastKnownTime = DateTime.Now;
    }
    
    private void onAppResumed()
    {
        App.WindowResumed -= onAppResumed;
        App.WindowActivated -= onAppResumed;

        App.WindowDeactivated += onAppMinimized;
        App.WindowStopped += onAppMinimized;
        
        if (_lastKnownTime != null)
        {
            TimeSpan timeElapsed = DateTime.Now - _lastKnownTime.Value;
            _lastKnownTime = null;

            TimeLeft -= timeElapsed.Seconds;
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

    protected void SetProperty<T>(ref T backingStore, in T value, [CallerMemberName] in string propertyname = "")
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
}
