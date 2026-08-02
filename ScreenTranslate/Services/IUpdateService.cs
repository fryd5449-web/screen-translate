using ScreenTranslate.Models;

namespace ScreenTranslate.Services
{
    public interface IUpdateService
    {
        UpdateInfo CheckForUpdates(string currentVersion);
    }
}