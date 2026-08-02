using ScreenTranslate.Models;

namespace ScreenTranslate.Services
{
    public class SettingsService : ISettingsService
    {
        private AppSettings _settings = new AppSettings();

        public AppSettings Load()
        {
            return _settings;
        }

        public void Save(AppSettings settings)
        {
            _settings = settings ?? new AppSettings();
        }
    }
}