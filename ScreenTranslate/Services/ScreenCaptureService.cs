using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using ScreenTranslate.Models;
using ScreenTranslate.Views;

namespace ScreenTranslate.Services
{
    public class ScreenCaptureService : IScreenCaptureService
    {
        public Task<ScreenCaptureResult> CaptureAreaAsync()
        {
            var taskSource = new TaskCompletionSource<ScreenCaptureResult>();
            var overlay = new SelectionOverlayWindow();

            overlay.SelectionCanceled += (s, e) =>
            {
                taskSource.TrySetResult(new ScreenCaptureResult { IsCancelled = true });
                overlay.Close();
            };

            overlay.SelectionCompleted += (s, rectDip) =>
            {
                var capture = CaptureRect(overlay, rectDip);
                taskSource.TrySetResult(capture);
                overlay.Close();
            };

            overlay.Show();
            return taskSource.Task;
        }

        private static ScreenCaptureResult CaptureRect(Window overlay, Rect selectionDip)
        {
            var source = PresentationSource.FromVisual(overlay);
            if (source == null || source.CompositionTarget == null)
            {
                return new ScreenCaptureResult { IsCancelled = true };
            }

            var transform = source.CompositionTarget.TransformToDevice;
            var pixelX = (int)Math.Round((overlay.Left + selectionDip.Left) * transform.M11);
            var pixelY = (int)Math.Round((overlay.Top + selectionDip.Top) * transform.M22);
            var pixelWidth = Math.Max(1, (int)Math.Round(selectionDip.Width * transform.M11));
            var pixelHeight = Math.Max(1, (int)Math.Round(selectionDip.Height * transform.M22));

            var bitmap = new Bitmap(pixelWidth, pixelHeight, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(pixelX, pixelY, 0, 0, new System.Drawing.Size(pixelWidth, pixelHeight), CopyPixelOperation.SourceCopy);
            }

            return new ScreenCaptureResult
            {
                IsCancelled = false,
                CapturedBitmap = bitmap,
                PixelBounds = new Rectangle(pixelX, pixelY, pixelWidth, pixelHeight)
            };
        }
    }
}