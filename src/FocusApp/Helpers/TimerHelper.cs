using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Helpers
{
    internal class TimerHelper
    {
        private int _counter = 0;
        private DateTime? lastKnownTime;
        private bool IsTimerActive = false;

        public TimerHelper()
        {
            onTimerStart();
        }

        private void onTimerStart()
        {
            App.WindowDeactivated += onAppMinimized;
            App.WindowStopped += onAppMinimized;

            IsTimerActive = true;
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

            _counter -= timeElapsed.Value.Seconds;
        }
    }
}
