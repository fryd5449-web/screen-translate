using System.Drawing;
using System.Threading.Tasks;
using ScreenTranslate.Models;

namespace ScreenTranslate.Services
{
    public interface IOcrService
    {
        Task<OcrResult> ExtractTextAsync(Bitmap image, string sourceLanguageCode);
    }
}