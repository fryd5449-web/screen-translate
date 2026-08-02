using System.Threading.Tasks;

namespace ScreenTranslate.Services
{
    public interface ITranslationService
    {
        Task<string> TranslateToArabicAsync(string text, string sourceLanguageCode);
    }
}