using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenTime.Types
{
    public class WindowsProcess
    {
        public WindowsProcess()
        {
            ClockPeriods = new List<ClockPeriod>();
        }

        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public string ProcessName { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool IsRunning { get; set; }
        public IntPtr Handle { get; set; }
        public readonly IEnumerable<ClockPeriod> ClockPeriods;
        public TimeSpan TotalTimeActive
        {
            get
            {
                return ClockPeriods.Select(w => (w.EndTime ?? DateTime.Now) - w.StartTime).Aggregate(new TimeSpan(), (current, next) => current + next);      
            }
        }

        public void SetNotActive()
        {
            IsActive = false;

            var timesToEnd = ClockPeriods.Where(w => w.EndTime == null);

            foreach (var timeToEnd in timesToEnd)
            {
                timeToEnd.EndTime = DateTime.Now;
            }
        }

        public void SetActive()
        {
            IsRunning = true;
            IsActive = true;
            ((List<ClockPeriod>)ClockPeriods).Add(new ClockPeriod());
        }
    }
}
