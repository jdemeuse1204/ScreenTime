using ScreenTime.Types;
using ScreenTime.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTime.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<ProcessRollup> Processes { get; set; }

        public string TotalTime { get; set; }

        public ProcessRollup SelectedProcess { get; set; }

        public DateTime DayStart { get; set; }

        public MainViewModel()
        {
            Processes = new ObservableCollection<ProcessRollup>();
        }
    }
}
