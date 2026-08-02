using System;
using System.Threading;
using System.Windows;
using ScreenTranslate.Services;
using ScreenTranslate.Views;
namespace ScreenTranslate
{
    public partial class App : Application
    {
        private const string MutexName = "ScreenTranslate.SingleInstance.6D328E98";
        private Mutex _singleInstanceMutex;
        private bool _ownsMutex;
        private MainWindow _mainWindow;
        private FloatingButtonWindow _floatingButton;
        public ISettingsService SettingsService { get; private set; }
        public ITrayIconService TrayIconService { get; private set; }
        public IGlobalHotkeyService GlobalHotkeyService { get; private set; }
        public IScreenCaptureService ScreenCaptureService { get; private set; }
        public IOcrService OcrService { get; private set; }
        public ITranslationService TranslationService { get; private set; }
        public IUpdateService UpdateService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            _singleInstanceMutex = new Mutex(true, MutexName, out _ownsMutex);
            if (!_ownsMutex) { Shutdown(); return; }
            base.OnStartup(e);
            SettingsService = new SettingsService(); TrayIconService = new TrayIconService(); GlobalHotkeyService = new GlobalHotkeyService(); ScreenCaptureService = new ScreenCaptureService(); OcrService = new OcrService(); TranslationService = new TranslationService(); UpdateService = new UpdateService();
            _mainWindow = new MainWindow(SettingsService, ScreenCaptureService, OcrService, TranslationService, UpdateService);
            _mainWindow.BackgroundNoticeRequested += OnBackgroundNoticeRequested;
            _mainWindow.SelectionActivityChanged += OnSelectionActivityChanged;
            _mainWindow.Show();
            _floatingButton = new FloatingButtonWindow(_mainWindow.SourceLanguages, _mainWindow.SelectedSourceLanguageCode);
            _floatingButton.CaptureRequested += OnTrayCaptureRequested;
            _floatingButton.OpenProgramRequested += OnTrayShowRequested;
            _floatingButton.LanguageRequested += OnFloatingLanguageRequested;
            _floatingButton.ExitRequested += OnTrayExitRequested;
            _floatingButton.Show();
            TrayIconService.Initialize();
            TrayIconService.ShowRequested += OnTrayShowRequested;
            TrayIconService.CaptureRequested += OnTrayCaptureRequested;
            TrayIconService.SettingsRequested += OnTraySettingsRequested;
            TrayIconService.ExitRequested += OnTrayExitRequested;
            GlobalHotkeyService.HotkeyPressed += OnHotkeyPressed;
            if (!GlobalHotkeyService.RegisterHotkey()) TrayIconService.ShowBalloon("مترجم الشاشة", "تعذر تسجيل الاختصار Ctrl + Shift + T.");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_mainWindow != null) { _mainWindow.BackgroundNoticeRequested -= OnBackgroundNoticeRequested; _mainWindow.SelectionActivityChanged -= OnSelectionActivityChanged; }
            if (_floatingButton != null) { _floatingButton.Close(); _floatingButton = null; }
            if (GlobalHotkeyService != null) GlobalHotkeyService.Dispose();
            if (TrayIconService != null) TrayIconService.Dispose();
            if (_singleInstanceMutex != null) { if (_ownsMutex) _singleInstanceMutex.ReleaseMutex(); _singleInstanceMutex.Dispose(); }
            base.OnExit(e);
        }

        private void OnTrayShowRequested(object sender, EventArgs e) { ShowMainWindow(); }
        private void OnTrayCaptureRequested(object sender, EventArgs e) { if (_mainWindow != null) _mainWindow.BeginSelectionFromExternalTrigger(); }
        private void OnTraySettingsRequested(object sender, EventArgs e) { ShowMainWindow(); if (_mainWindow != null) _mainWindow.OpenSettingsWindow(); }
        private void OnHotkeyPressed(object sender, EventArgs e) { ShowMainWindow(); }
        private void OnBackgroundNoticeRequested(object sender, string message) { if (TrayIconService != null) TrayIconService.ShowBalloon("مترجم الشاشة", message); }
        private void OnSelectionActivityChanged(object sender, bool isActive) { if (_floatingButton == null) return; if (isActive) _floatingButton.Hide(); else _floatingButton.Show(); }
        private void OnFloatingLanguageRequested(object sender, string languageCode) { if (_mainWindow != null) _mainWindow.SetSourceLanguage(languageCode); }
        private void ShowMainWindow() { if (_mainWindow == null) return; _mainWindow.Show(); _mainWindow.WindowState = WindowState.Normal; _mainWindow.Activate(); }
        private void OnTrayExitRequested(object sender, EventArgs e) { if (_mainWindow != null) _mainWindow.AllowCloseForExit(); Shutdown(); }
    }
}
