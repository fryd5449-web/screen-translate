using System;
using System.Drawing;

namespace ScreenTranslate.Models
{
    public class ScreenCaptureResult : IDisposable
    {
        public bool IsCancelled { get; set; }

        public Bitmap CapturedBitmap { get; set; }

        public Rectangle PixelBounds { get; set; }

        public void Dispose()
        {
            if (CapturedBitmap != null)
            {
                CapturedBitmap.Dispose();
            }

            CapturedBitmap = null;
        }
    }
}