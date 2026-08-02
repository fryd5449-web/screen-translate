using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Forms = System.Windows.Forms;

namespace ScreenTranslate.Views
{
    public partial class SelectionOverlayWindow : Window
    {
        private const int MinSelectionPixels = 10;

        private System.Windows.Point _startPoint;
        private bool _isDragging;
        private Rect _currentRect;

        public SelectionOverlayWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public event EventHandler<Rect> SelectionCompleted;

        public event EventHandler SelectionCanceled;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ConfigureWindowForVirtualScreen();
            UpdateMaskGeometry(Rect.Empty);
            Activate();
            Focus();
            Keyboard.Focus(RootGrid);
        }

        private void ConfigureWindowForVirtualScreen()
        {
            var virtualBounds = Forms.SystemInformation.VirtualScreen;
            var dpi = VisualTreeHelper.GetDpi(this);

            Left = virtualBounds.Left / dpi.DpiScaleX;
            Top = virtualBounds.Top / dpi.DpiScaleY;
            Width = virtualBounds.Width / dpi.DpiScaleX;
            Height = virtualBounds.Height / dpi.DpiScaleY;
        }

        private void RootGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _startPoint = e.GetPosition(this);
            _currentRect = Rect.Empty;

            SelectionRectangle.Visibility = Visibility.Visible;
            SelectionSizeBadge.Visibility = Visibility.Visible;
            Canvas.SetLeft(SelectionRectangle, _startPoint.X);
            Canvas.SetTop(SelectionRectangle, _startPoint.Y);
            SelectionRectangle.Width = 0;
            SelectionRectangle.Height = 0;
            Canvas.SetLeft(SelectionSizeBadge, _startPoint.X + 8);
            Canvas.SetTop(SelectionSizeBadge, _startPoint.Y + 8);
            SelectionSizeText.Text = "0 x 0";
            HintText.Text = "اسحب لتحديد النص - Esc للإلغاء";
            Mouse.Capture(RootGrid);
        }

        private void RootGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
            {
                return;
            }

            var current = e.GetPosition(this);
            var left = Math.Min(current.X, _startPoint.X);
            var top = Math.Min(current.Y, _startPoint.Y);
            var width = Math.Abs(current.X - _startPoint.X);
            var height = Math.Abs(current.Y - _startPoint.Y);

            _currentRect = new Rect(left, top, width, height);

            Canvas.SetLeft(SelectionRectangle, left);
            Canvas.SetTop(SelectionRectangle, top);
            SelectionRectangle.Width = width;
            SelectionRectangle.Height = height;

            UpdateMaskGeometry(_currentRect);
            UpdateSelectionBadge(left, top, width, height);
        }

        private void RootGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
            {
                return;
            }

            _isDragging = false;
            Mouse.Capture(null);

            var left = Canvas.GetLeft(SelectionRectangle);
            var top = Canvas.GetTop(SelectionRectangle);
            var width = SelectionRectangle.Width;
            var height = SelectionRectangle.Height;

            var selectionRectDip = new Rect(left, top, width, height);
            if (!IsSelectionLargeEnough(selectionRectDip))
            {
                HintText.Text = "الحد الأدنى للتحديد هو 10 × 10 بكسل";
                ResetSelectionVisuals();
                return;
            }

            if (SelectionCompleted != null)
            {
                SelectionCompleted(this, selectionRectDip);
            }
        }

        private void RootGrid_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMaskGeometry(_currentRect);
        }

        private void RootGrid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelSelection();
            }
        }

        private void CancelSelection()
        {
            ResetSelectionVisuals();

            if (SelectionCanceled != null)
            {
                SelectionCanceled(this, EventArgs.Empty);
            }
        }

        private void UpdateSelectionBadge(double left, double top, double width, double height)
        {
            var dpi = VisualTreeHelper.GetDpi(this);
            var widthPx = Math.Max(0, (int)Math.Round(width * dpi.DpiScaleX));
            var heightPx = Math.Max(0, (int)Math.Round(height * dpi.DpiScaleY));
            SelectionSizeText.Text = widthPx + " x " + heightPx;

            var badgeLeft = Math.Max(0, left + 8);
            var badgeTop = Math.Max(0, top - 34);

            Canvas.SetLeft(SelectionSizeBadge, badgeLeft);
            Canvas.SetTop(SelectionSizeBadge, badgeTop);
        }

        private void UpdateMaskGeometry(Rect clearRect)
        {
            var fullRect = new RectangleGeometry(new Rect(0, 0, Math.Max(0, ActualWidth), Math.Max(0, ActualHeight)));
            var group = new GeometryGroup();
            group.FillRule = FillRule.EvenOdd;
            group.Children.Add(fullRect);

            if (!clearRect.IsEmpty && clearRect.Width > 0 && clearRect.Height > 0)
            {
                group.Children.Add(new RectangleGeometry(clearRect));
            }

            DimPath.Data = group;
        }

        private bool IsSelectionLargeEnough(Rect rectDip)
        {
            var dpi = VisualTreeHelper.GetDpi(this);
            var widthPx = (int)Math.Round(rectDip.Width * dpi.DpiScaleX);
            var heightPx = (int)Math.Round(rectDip.Height * dpi.DpiScaleY);
            return widthPx >= MinSelectionPixels && heightPx >= MinSelectionPixels;
        }

        private void ResetSelectionVisuals()
        {
            _isDragging = false;
            _currentRect = Rect.Empty;
            SelectionRectangle.Visibility = Visibility.Collapsed;
            SelectionSizeBadge.Visibility = Visibility.Collapsed;
            UpdateMaskGeometry(Rect.Empty);
        }
    }
}