using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FocusApp.Helpers;

internal class TimerHelper : INotifyPropertyChanged
{   
    public class Timer
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

    private Timer _timerDisplay;
    public Timer TimerDisplay
    {
        get => _timerDisplay;
        set => SetProperty(ref _timerDisplay, value);
    }

    private int _timeLeft;

    private DateTime? lastKnownTime;
    private bool IsTimerActive = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Action<int>? StartTimer { get; set; }

    public Action? CancelTimer { get; set; }

    public TimerHelper()
    {
        _timerDisplay = new Timer();
        StartTimer += onTimerStart;
    }

    #region App Lifecycle Triggered Logic

    private void onTimerStart(int timerSeconds)
    {
        App.WindowDeactivated += onAppMinimized;
        App.WindowStopped += onAppMinimized;

        _timeLeft = timerSeconds;
        IsTimerActive = true;

        var timer = Application.Current?.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(1000);
        timer.Tick += (sender, eventArgs) =>
        {
            _timeLeft--;
            TimerDisplay = new Timer
            {
                HourTime = (int)Math.Floor(_timeLeft / 3600f),
                MinuteTime = ((int)Math.Floor(_timeLeft / 60f)) % 60,
                SecondTime = _timeLeft % 60
            };
        };
        timer.Start();
    }

    private void onTimerStop()
    {
        IsTimerActive = false;
    }

    private void onAppMinimized()
    {
        App.WindowDeactivated -= onAppMinimized;
        App.WindowStopped -= onAppMinimized;

        App.WindowActivated += onAppResumed;

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
