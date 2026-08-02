namespace ScreenTranslate.Models
{
    public class UpdateInfo
    {
        public string CurrentVersion { get; set; }

        public bool IsUpdateAvailable { get; set; }

        public string LatestVersion { get; set; }

        public string ReleaseNotes { get; set; }

        public string UpdateSize { get; set; }

        public string TrustedDownloadUrl { get; set; }

        public string StatusMessageArabic { get; set; }
    }
}