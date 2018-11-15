using Microsoft.Win32;
using Newtonsoft.Json;
using ScreenTime.Managers;
using ScreenTime.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace ScreenTime
{
    public class Processor : IDisposable
    {
        private readonly SessionSwitchEventHandler SessionSwitchEventHandler;
        private readonly ProcessManager ProcessManager;
        private bool IsSessionLocked { get; set; }
        private List<WindowsProcess> Processes { get; set; }
        public IEnumerable<WindowsProcess> WindowsProcesses { get { return Processes; } }

        public Processor()
        {
            Processes = LoadInternalProcesses();
            IsSessionLocked = false;
            ProcessManager = new ProcessManager();
            SessionSwitchEventHandler = new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            SystemEvents.SessionSwitch += SessionSwitchEventHandler;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                IsSessionLocked = true;
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                IsSessionLocked = false;
            }
        }

        public void Run()
        {
            if (IsSessionLocked == true)
            {
                // end all processes that are running
                foreach (var process in Processes)
                {
                    process.SetNotActive();
                }
                
                return;
            }

            var windowsProcesses = ProcessManager.GetAllProcesses();

            // check to see if any processes were closed
            foreach (var process in Processes)
            {
                if (process.IsActive && windowsProcesses.Any(w => w.Id == process.Id) == false)
                {
                    // process was closed, deactivate it
                    process.IsRunning = false;
                    process.SetNotActive();
                }
            }

            foreach (var process in windowsProcesses)
            {
                var foundProcess = Processes.FirstOrDefault(w => w.Id == process.Id);

                if (foundProcess == null)
                {
                    // new process has started
                    if (process.IsActive)
                    {
                        // if its active, mark as active 
                        process.SetActive();
                    }

                    Processes.Add(process);
                }
                else
                {
                    // update it
                    if (process.IsActive && foundProcess.IsActive == false)
                    {
                        foundProcess.SetActive();
                    }
                    else if (foundProcess.IsActive == true && process.IsActive == false)
                    {
                        foundProcess.SetNotActive();
                    }
                }
            }
        }

        private void SaveInternalProcesses(List<WindowsProcess> processes)
        {
            var path = GetFileLocation();
            var directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(GetFileLocation(), JsonConvert.SerializeObject(processes));
        }

        private string GetFileLocation()
        {
            var x = ConfigurationManager.AppSettings["LogFileLocation"];

            return string.Format(ConfigurationManager.AppSettings["LogFileLocation"], Environment.UserName, "ScreenTime.json");
        }

        private List<WindowsProcess> LoadInternalProcesses()
        {
            if (!File.Exists(GetFileLocation()))
            {
                return new List<WindowsProcess>();
            }

            var text = File.ReadAllText(GetFileLocation());

            if (string.IsNullOrEmpty(text))
            {
                return new List<WindowsProcess>();
            }

            return JsonConvert.DeserializeObject<List<WindowsProcess>>(text);
        }

        public void Dispose()
        {
            foreach (var process in Processes)
            {
                process.IsRunning = false;
                process.SetNotActive();
            }

            SaveInternalProcesses(Processes);

            SystemEvents.SessionSwitch -= SessionSwitchEventHandler;
        }
    }
}
