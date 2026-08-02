using ScreenTranslate.Models;

namespace ScreenTranslate.Services
{
    public interface ISettingsService
    {
        AppSettings Load();

        void Save(AppSettings settings);
    }
}