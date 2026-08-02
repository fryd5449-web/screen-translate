using System;

namespace ScreenTranslate.Services
{
    public interface IGlobalHotkeyService : IDisposable
    {
        event EventHandler HotkeyPressed;

        bool RegisterHotkey();

        void UnregisterHotkey();
    }
}