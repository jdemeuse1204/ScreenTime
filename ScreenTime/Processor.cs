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
        private List<ProcessRollup> ProcessRollups { get; set; }
        public IEnumerable<ProcessRollup> WindowsProcessRollups { get { return ProcessRollups; } }

        public Processor()
        {
            ProcessRollups = LoadInternalProcesses();
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
            var processes = ProcessRollups.SelectMany(w => w.Processes);

            if (IsSessionLocked == true)
            {
                // end all processes that are running
                foreach (var process in processes)
                {
                    process.SetNotActive();
                }
                
                return;
            }

            var windowsProcesses = ProcessManager.GetAllProcesses();

            // check to see if any processes were closed
            foreach (var process in processes)
            {
                if (process.IsActive && windowsProcesses.Any(w => w.Key == process.Key) == false)
                {
                    // process was closed, deactivate it
                    process.IsRunning = false;
                    process.SetNotActive();
                }
            }

            foreach (var process in windowsProcesses)
            {
                var rollup = ProcessRollups.FirstOrDefault(w => string.Equals(w.ProcessName, process.ProcessName, StringComparison.OrdinalIgnoreCase));

                if (rollup == null)
                {
                    rollup = new ProcessRollup(process.ProcessName);
                    ProcessRollups.Add(rollup);
                }

                var foundProcess = rollup.Processes.FirstOrDefault(w => w.Key == process.Key);

                if (foundProcess == null)
                {
                    // new process has started
                    if (process.IsActive)
                    {
                        // if its active, mark as active 
                        process.SetActive();
                    }

                    rollup.Add(process);
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

        private void SaveInternalProcesses(List<ProcessRollup> processes)
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

        private List<ProcessRollup> LoadInternalProcesses()
        {
            if (!File.Exists(GetFileLocation()))
            {
                return new List<ProcessRollup>();
            }

            var text = File.ReadAllText(GetFileLocation());

            if (string.IsNullOrEmpty(text))
            {
                return new List<ProcessRollup>();
            }

            return JsonConvert.DeserializeObject<List<ProcessRollup>>(text);
        }

        public void Dispose()
        {
            foreach (var process in ProcessRollups.SelectMany(w => w.Processes))
            {
                process.IsRunning = false;
                process.SetNotActive();
            }

            SaveInternalProcesses(ProcessRollups);

            SystemEvents.SessionSwitch -= SessionSwitchEventHandler;
        }
    }
}
