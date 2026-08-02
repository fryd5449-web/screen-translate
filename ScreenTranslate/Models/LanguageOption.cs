namespace ScreenTranslate.Models
{
    public class LanguageOption
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}