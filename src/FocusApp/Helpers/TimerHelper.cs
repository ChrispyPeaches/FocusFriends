using CommunityToolkit.Maui.Markup;
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
    private int _timeLeft;
    private IDispatcherTimer? _timer;
    private DateTime? lastKnownTime;
    private TimerState _state = TimerState.StoppedPreStudy;

    #endregion

    #region Properties

    public TimerDto TimerDisplay
    {
        get => _timerDisplay;
        set => SetProperty(ref _timerDisplay, value);
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
    public Action<Button> ToggleTimer { get; set; }

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
        ToggleTimer += onTimerToggleButtonClick;
    }

    public void onTimeStepperButtonClick(TimerView.TimerButton clickedButton)
    {
        if (clickedButton == TimerView.TimerButton.Up)
        {
            TimeLeft += 60;
        }
        else
        {
            TimeLeft -= 60;
        }
    }

    private void onTimerToggleButtonClick(Button buttonTimerToggle)
    {
        TransitionToNextState();

        switch (_state)
        {
            case TimerState.StoppedPreStudy:
                onTimerStop();
                buttonTimerToggle.Text = "Start Studying";
                break;

            case TimerState.StudyCountdown:
                onTimerStart();
                buttonTimerToggle.Text = "Stop";
                break;

            case TimerState.StoppedPreBreak:
                onTimerStop();
                buttonTimerToggle.Text = "Start Break";
                break;

            case TimerState.BreakCountdown:
                onTimerStart();
                buttonTimerToggle.Text = "Skip";
                break;
        }
    }

    private void TransitionToNextState()
    {
        switch (_state)
        {
            case TimerState.StoppedPreStudy:
                _state = TimerState.StudyCountdown;
                break;

            case TimerState.StudyCountdown:
                _state = _timeLeft > 0 ?
                    TimerState.StoppedPreStudy
                    : TimerState.StoppedPreBreak;
                break;

            case TimerState.StoppedPreBreak:
                _state = TimerState.BreakCountdown;
                break;

            case TimerState.BreakCountdown:
                _state = TimerState.StoppedPreStudy;
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

        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(1000);
        _timer.Tick += (sender, eventArgs) =>
        {
            TimeLeft--;
            if (TimeLeft <= 0)
            {
                onTimerStop();
            }
        };
        _timer.Start();
    }


    private void onTimerStop()
    {
        _timeLeft = 0;
        if (_timer != null)
        {
            _timer.Stop();
        }

        TimerDisplay = new TimerDto
        {
            HourTime = 0,
            MinuteTime = 0,
            SecondTime = 0
        };
    }

    #region App Lifecycle Triggered Logic

    private void onAppMinimized()
    {
        App.WindowDeactivated -= onAppMinimized;
        App.WindowStopped -= onAppMinimized;

        App.WindowActivated += onAppResumed;
        App.WindowResumed += onAppResumed;

        lastKnownTime = DateTime.Now;
    }
    
    private void onAppResumed()
    {
        App.WindowResumed -= onAppResumed;
        App.WindowActivated -= onAppResumed;

        App.WindowDeactivated += onAppMinimized;
        App.WindowStopped += onAppMinimized;
        
        TimeSpan? timeElapsed = null;
        if (lastKnownTime != null)
        {
            timeElapsed = DateTime.Now - lastKnownTime.Value;
            lastKnownTime = null;
        }

        if (_state != TimerState.StoppedPreStudy && timeElapsed != null)
        {
            _timeLeft -= timeElapsed.Value.Seconds;
            if (_timeLeft <= 0)
            {
                onTimerStop();
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
