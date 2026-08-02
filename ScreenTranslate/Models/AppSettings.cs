namespace ScreenTranslate.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            SourceLanguageCode = "auto";
        }

        public bool StartWithWindows { get; set; }

        public bool KeepTranslationOnTop { get; set; }

        public string SourceLanguageCode { get; set; }
    }
}