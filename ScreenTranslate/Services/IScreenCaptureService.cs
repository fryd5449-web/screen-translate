using System.Threading.Tasks;
using ScreenTranslate.Models;

namespace ScreenTranslate.Services
{
    public interface IScreenCaptureService
    {
        Task<ScreenCaptureResult> CaptureAreaAsync();
    }
}