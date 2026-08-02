using System.Reflection;
using System.Windows;
using ScreenTranslate.Models;
using ScreenTranslate.Services;

namespace ScreenTranslate.Views
{
    public partial class AboutUpdatesWindow : Window
    {
        private readonly IUpdateService _updateService;
        private readonly string _currentVersion;

        public AboutUpdatesWindow(IUpdateService updateService)
        {
            _updateService = updateService;
            _currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            InitializeComponent();

            CurrentVersionText.Text = _currentVersion;
            FillFutureFields(_updateService.CheckForUpdates(_currentVersion));
        }

        private void CheckForUpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var info = _updateService.CheckForUpdates(_currentVersion);
            FillFutureFields(info);
            MessageBox.Show(info.StatusMessageArabic, "فحص التحديثات", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FillFutureFields(UpdateInfo info)
        {
            LatestVersionText.Text = string.IsNullOrWhiteSpace(info.LatestVersion) ? "-" : info.LatestVersion;
            UpdateSizeText.Text = string.IsNullOrWhiteSpace(info.UpdateSize) ? "-" : info.UpdateSize;
            ReleaseNotesText.Text = string.IsNullOrWhiteSpace(info.ReleaseNotes) ? "-" : info.ReleaseNotes;
            TrustedUrlText.Text = string.IsNullOrWhiteSpace(info.TrustedDownloadUrl) ? "-" : info.TrustedDownloadUrl;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}