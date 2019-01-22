using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using ScreenTime.Managers;
using ScreenTime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ScreenTime
{
    public class Processor : IDisposable
    {
        private IKeyboardMouseEvents m_GlobalHook;
        private readonly SessionSwitchEventHandler SessionSwitchEventHandler;
        private readonly ProcessManager ProcessManager;
        private readonly RollupManager RollupManager;
        private DateTime LastActivityDate { get; set; }

        private bool IsSessionLocked { get; set; }
        private List<ProcessRollup> ProcessRollups { get; set; }
        public IEnumerable<ProcessRollup> WindowsProcessRollups { get { return ProcessRollups; } }

        public Processor()
        {
            RollupManager = new RollupManager();
            ProcessManager = new ProcessManager();

            var savedData = ProcessManager.GetLoggedProcesses();

            ProcessRollups = RollupManager.RollupProcesses(savedData).ToList();
            IsSessionLocked = false;
            SessionSwitchEventHandler = new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            SystemEvents.SessionSwitch += SessionSwitchEventHandler;

            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress += GlobalHookKeyPress;
            m_GlobalHook.MouseMove += M_GlobalHook_MouseMove;

            LastActivityDate = DateTime.Now;
        }

        private void M_GlobalHook_MouseMove(object sender, MouseEventArgs e)
        {
            LastActivityDate = DateTime.Now;
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            LastActivityDate = DateTime.Now;
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            LastActivityDate = DateTime.Now;
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
            if ((LastActivityDate - DateTime.Now).TotalMinutes >= 5)
            {
                // user is away
            }

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

            var windowsProcesses = ProcessManager.GetCurrentProcesses();

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

        public void Dispose()
        {
            var windowsProcesses = ProcessRollups.SelectMany(w => w.Processes);

            foreach (var process in windowsProcesses)
            {
                process.IsRunning = false;
                process.SetNotActive();
            }

            ProcessManager.SaveProcesses(windowsProcesses);

            SystemEvents.SessionSwitch -= SessionSwitchEventHandler;
        }
    }
}
