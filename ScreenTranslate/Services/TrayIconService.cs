using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace ScreenTranslate.Services
{
    public sealed class TrayIconService : ITrayIconService
    {
        private NotifyIcon _notifyIcon;
        private Icon _appIcon;
        public event EventHandler ShowRequested;
        public event EventHandler CaptureRequested;
        public event EventHandler SettingsRequested;
        public event EventHandler ExitRequested;

        public void Initialize()
        {
            if (_notifyIcon != null) return;
            var menu = new ContextMenuStrip { ShowImageMargin = false, BackColor = Color.FromArgb(24, 30, 44), ForeColor = Color.White };
            AddItem(menu, "تحديد نص", CaptureRequested);
            AddItem(menu, "فتح البرنامج", ShowRequested);
            AddItem(menu, "الإعدادات", SettingsRequested);
            menu.Items.Add(new ToolStripSeparator());
            AddItem(menu, "خروج", ExitRequested);
            _appIcon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "app.ico"));
            _notifyIcon = new NotifyIcon { Icon = _appIcon, Visible = true, Text = "مترجم الشاشة", ContextMenuStrip = menu };
            _notifyIcon.MouseClick += OnMouseClick;
        }

        private void AddItem(ContextMenuStrip menu, string text, EventHandler handler)
        {
            menu.Items.Add(text, null, delegate { if (handler != null) handler(this, EventArgs.Empty); });
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CaptureRequested != null) CaptureRequested(this, EventArgs.Empty);
        }

        public void ShowBalloon(string title, string text)
        {
            if (_notifyIcon != null) _notifyIcon.ShowBalloonTip(2600, title, text, ToolTipIcon.Info);
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.MouseClick -= OnMouseClick;
                _notifyIcon.Visible = false;
                if (_notifyIcon.ContextMenuStrip != null) _notifyIcon.ContextMenuStrip.Dispose();
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
            if (_appIcon != null) { _appIcon.Dispose(); _appIcon = null; }
        }

    }
}
