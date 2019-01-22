using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ScreenTime.Managers
{
    public class WindowsEventManager : IDisposable
    {
        private readonly IKeyboardMouseEvents m_GlobalHook = Hook.GlobalEvents();
        private KeyPressEventHandler KeyPressEventHandler { get; set; }
        private MouseEventHandler MouseEventHandler { get; set; }
        private EventHandler<MouseEventExtArgs> MouseDownEventHandler { get; set; }
        private SessionSwitchEventHandler SessionSwitchEventHandler { get; set; }

        private void RegisterGlobalKeyPressHandler(KeyPressEventHandler handler)
        {
            KeyPressEventHandler = handler;
            m_GlobalHook.KeyPress += KeyPressEventHandler;
        }

        private void RegisterGlobalMouseMoveHandler(MouseEventHandler handler)
        {
            MouseEventHandler = handler;
            m_GlobalHook.MouseMove += MouseEventHandler;
        }
        private void RegisterGlobalMouseDownHandler(EventHandler<MouseEventExtArgs> handler)
        {
            MouseDownEventHandler = handler;
            m_GlobalHook.MouseDownExt += MouseDownEventHandler;
        }

        private void RegisterSessionSwitchedHander(SessionSwitchEventHandler handler)
        {
            SessionSwitchEventHandler = handler;
            SystemEvents.SessionSwitch += SessionSwitchEventHandler;
        }

        public void Dispose()
        {
            SystemEvents.SessionSwitch -= SessionSwitchEventHandler;
            m_GlobalHook.MouseDownExt -= MouseDownEventHandler;
            m_GlobalHook.MouseMove -= MouseEventHandler;
            m_GlobalHook.KeyPress -= KeyPressEventHandler;
        }
    }
}
