using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace PendingOrdersGoalTracking
{
    public delegate void TimeReachedEventHandler(DateTime Time);

    /// <summary>
    /// A class that can detect when a new day is detected.
    /// </summary>
    public class DailyTimer
    {
        private static Timer _timer = null;

        /// <summary>
        /// An event that happens at midnight
        /// </summary>
        public event TimeReachedEventHandler NewDay;

        /// <summary>
        /// Initializes a new DailyTimer instance and starts waiting for a new day.
        /// </summary>
        public DailyTimer()
        {
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            Start();
        }

        /// <summary>
        /// Starts the Timer to fire at midnight, every night (based on server time).
        /// </summary>
        public void Start()
        {
            TimeSpan timeToWait = Tomorrow.Subtract(DateTime.Now);

            if (timeToWait.TotalMilliseconds > 0)
            {
                _timer.Interval = timeToWait.TotalMilliseconds;
                _timer.Start();
            }
            else
            {
                SendNewDay();
            }
        }

        public void Stop()
        {
            if (_timer != null)
                _timer.Stop();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                SendNewDay();
            }
            catch
            {
            }
        }

        private void SendNewDay()
        {
            try
            {
                // Stop the orginal timer and restart for the new day
                Stop();
                // now raise a event
                OnNewDay(DateTime.Today);                
            }
            finally
            {
                Start();
            }
        }

        private DateTime Tomorrow
        {
            get
            {
                return DateTime.Today.AddDays(1);
            }
        }

        /// <summary>
        /// Fires the new day event
        /// </summary>
        private void OnNewDay(DateTime day)
        {
            if (this.NewDay != null)
            {
                this.NewDay(day);
            }
        }
    }
}
