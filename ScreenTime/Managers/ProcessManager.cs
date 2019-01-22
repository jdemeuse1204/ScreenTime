using ScreenTime.Repositories;
using ScreenTime.Types;
using ScreenTime.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;

namespace ScreenTime.Managers
{
    public class ProcessManager
    {
        private readonly ProcessRepository ProcessRepository;

        public ProcessManager()
        {
            ProcessRepository = new ProcessRepository();
        }

        public IEnumerable<WindowsProcess> GetCurrentProcesses()
        {
            var activeWindowHandle = WindowsApi.GetforegroundWindow();
            var activeWindowId = WindowsApi.GetWindowProcessId(activeWindowHandle);

            return Process.GetProcesses().Where(w => !string.IsNullOrEmpty(w.MainWindowTitle)).Select(w => 
            {
                return new WindowsProcess(w.MainWindowTitle, w.ProcessName, w.Id)
                {
                    IsActive = activeWindowId == w.Id,
                    Handle = w.MainWindowHandle,
                    StartTime = w.StartTime,
                    IsRunning = true
                };
            });
        }

        public IEnumerable<WindowsProcess> GetLoggedProcesses()
        {
            return ProcessRepository.LoadSavedProcesses();
        }

        public void SaveProcesses(IEnumerable<WindowsProcess> processes)
        {
            ProcessRepository.Save(processes);
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
