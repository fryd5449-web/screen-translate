using System;
namespace ScreenTranslate.Services
{
    public interface ITrayIconService : IDisposable
    {
        event EventHandler ShowRequested;
        event EventHandler CaptureRequested;
        event EventHandler SettingsRequested;
        event EventHandler ExitRequested;
        void Initialize();
        void ShowBalloon(string title, string text);
    }
}
