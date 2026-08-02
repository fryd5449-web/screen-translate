using System;
using System.Windows;
using System.Windows.Input;
namespace ScreenTranslate.Views
{
    public partial class TranslationWindow : Window
    {
        public TranslationWindow() { InitializeComponent(); }
        public event EventHandler RetryRequested;
        public void UpdateContent(string original, string translated, string detectedArabic, string sourceArabic, string targetArabic) { TranslatedText.Text = translated; }
        private void CopyTranslatedButton_OnClick(object sender, RoutedEventArgs e) { if (!string.IsNullOrWhiteSpace(TranslatedText.Text)) Clipboard.SetText(TranslatedText.Text); }
        private void RetryButton_OnClick(object sender, RoutedEventArgs e) { if (RetryRequested != null) RetryRequested(this, EventArgs.Empty); }
        private void CloseButton_OnClick(object sender, RoutedEventArgs e) { Close(); }
        private void RootBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) { if (e.ChangedButton == MouseButton.Left) DragMove(); }
    }
}
