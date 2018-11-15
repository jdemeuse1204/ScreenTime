using Newtonsoft.Json;
using ScreenTime.Types;
using ScreenTime.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace ScreenTime.Managers
{
    public class ProcessManager
    {
        public IEnumerable<WindowsProcess> GetAllProcesses()
        {
            var activeWindowHandle = WindowsApi.GetforegroundWindow();
            var activeWindowId = WindowsApi.GetWindowProcessId(activeWindowHandle);
            return Process.GetProcesses().Where(w => !string.IsNullOrEmpty(w.MainWindowTitle)).Select(w => 
            {
                return new WindowsProcess
                {
                    IsActive = activeWindowId == w.Id,
                    Handle = w.MainWindowHandle,
                    Id = w.Id,
                    ProcessName = w.ProcessName,
                    StartTime = w.StartTime,
                    Title = w.MainWindowTitle,
                    IsRunning = true
                };
            });
        }

        public IEnumerable<WindowsProcess> GetLoggedProcesses()
        {
            var path = @"C:\Users\JamesDeMuse\Desktop\ScreenTime.txt";
            var todaysProcesses = string.Empty;

            if (File.Exists(path))
            {
                todaysProcesses = File.ReadAllText(path);
            }
    
            return JsonConvert.DeserializeObject<IEnumerable<WindowsProcess>>($"[{todaysProcesses}]");
        }

        private string GetChromeTabs(Process process)
        {
            if (process.MainWindowHandle == IntPtr.Zero)
            {
                return "";
            }

            AutomationElement root = AutomationElement.FromHandle(process.MainWindowHandle);
            var SearchBar = root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
            if (SearchBar != null)
            {
                var x = (string)SearchBar.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
            }

            return "";
        }
    }
}
