using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenTranslate.Services
{
    public class GlobalHotkeyService : IGlobalHotkeyService
    {
        private const int HotkeyId = 0x7411;
        private const int WmHotkey = 0x0312;
        private const uint ModControl = 0x0002;
        private const uint ModShift = 0x0004;
        private const uint VkT = 0x54;

        private readonly HotkeyMessageWindow _window;
        private bool _isRegistered;

        public GlobalHotkeyService()
        {
            _window = new HotkeyMessageWindow();
            _window.HotkeyMessageReceived += HandleHotkeyMessage;
        }

        public event EventHandler HotkeyPressed;

        public bool RegisterHotkey()
        {
            if (_isRegistered)
            {
                return true;
            }

            _isRegistered = RegisterHotKey(_window.Handle, HotkeyId, ModControl | ModShift, VkT);
            return _isRegistered;
        }

        public void UnregisterHotkey()
        {
            if (!_isRegistered)
            {
                return;
            }

            UnregisterHotKey(_window.Handle, HotkeyId);
            _isRegistered = false;
        }

        public void Dispose()
        {
            UnregisterHotkey();
            _window.HotkeyMessageReceived -= HandleHotkeyMessage;
            _window.Dispose();
        }

        private void HandleHotkeyMessage(object sender, int message)
        {
            if (message == WmHotkey)
            {
                if (HotkeyPressed != null)
                {
                    HotkeyPressed(this, EventArgs.Empty);
                }
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private sealed class HotkeyMessageWindow : NativeWindow, IDisposable
        {
            public event EventHandler<int> HotkeyMessageReceived;

            public HotkeyMessageWindow()
            {
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                if (HotkeyMessageReceived != null)
                {
                    HotkeyMessageReceived(this, m.Msg);
                }

                base.WndProc(ref m);
            }

            public void Dispose()
            {
                DestroyHandle();
            }
        }
    }
}