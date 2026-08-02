using ScreenTranslate.Models;

namespace ScreenTranslate.Services
{
    public class UpdateService : IUpdateService
    {
        public UpdateInfo CheckForUpdates(string currentVersion)
        {
            return new UpdateInfo
            {
                CurrentVersion = currentVersion,
                IsUpdateAvailable = false,
                LatestVersion = "-",
                ReleaseNotes = "سيتم عرض قائمة التغييرات هنا عند تفعيل نظام التحديثات.",
                UpdateSize = "-",
                TrustedDownloadUrl = "-",
                StatusMessageArabic = "أنت تستخدم أحدث إصدار."
            };
        }
    }
}