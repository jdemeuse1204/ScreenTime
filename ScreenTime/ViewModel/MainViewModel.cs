using ScreenTime.Extensions;
using ScreenTime.Managers;
using ScreenTime.Types;
using ScreenTime.ViewModel.Base;
using System;
using System.Collections.ObjectModel;

namespace ScreenTime.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<ProcessRollup> Processes { get; set; }

        public string TotalTime { get; set; }

        public ProcessRollup SelectedProcess { get; set; }

        public DateTime DayStart { get; set; }
    }
}
