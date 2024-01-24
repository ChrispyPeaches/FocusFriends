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

    private TimerDto _timerDisplay;
    public TimerDto TimerDisplay
    {
        get => _timerDisplay;
        set => SetProperty(ref _timerDisplay, value);
    }

    private int _timeLeft;
    private int TimeLeft
    {
        get => _timeLeft;
        set
        { 
            SetProperty(ref _timeLeft, value);
            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        TimerDisplay = new TimerDto
        {
            HourTime = (int)Math.Floor(TimeLeft / 3600f),
            MinuteTime = ((int)Math.Floor(TimeLeft / 60f)) % 60,
            SecondTime = TimeLeft % 60
        };
    }

    private IDispatcherTimer? _timer;

    private DateTime? lastKnownTime;
    private bool IsTimerActive = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Action? StartTimer { get; set; }

    public Action? CancelTimer { get; set; }

    public TimerHelper()
    {
        _timerDisplay = new TimerDto();
        StartTimer += onTimerStart;
        CancelTimer += onTimerStop;
    }

    public void onTimerButtonClick(TimerView.TimerButton clickedButton)
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

    private void onTimerStart()
    {
        App.WindowDeactivated += onAppMinimized;
        App.WindowStopped += onAppMinimized;
        App.WindowDestroying += onAppMinimized;

        TimeLeft =
            (TimerDisplay.HourTime * 3600) +
            (TimerDisplay.MinuteTime * 60) +
             TimerDisplay.SecondTime;

        IsTimerActive = true;

        _timer = Application.Current?.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(1000);
        _timer.Tick += (sender, eventArgs) =>
        {
            TimeLeft--;
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
        IsTimerActive = false;
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

        if (IsTimerActive && timeElapsed != null)
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
