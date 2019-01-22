using ScreenTime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTime.Managers
{
    public class RollupManager
    {
        public IEnumerable<ProcessRollup> RollupProcesses(IEnumerable<WindowsProcess> processes)
        {
            return processes.GroupBy(w => w.ProcessName).Select(w => new ProcessRollup(w.Key, w.ToList()));
        }
    }
}
