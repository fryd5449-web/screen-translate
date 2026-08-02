using System;
using System.Windows;
using System.Windows.Input;
namespace ScreenTranslate.Views
{
    public partial class ProcessingWindow : Window
    {
        public ProcessingWindow(){InitializeComponent(); Loaded+=(s,e)=>{Activate();Focus();};}
        public event EventHandler CancelRequested;
        public void SetTranslating(){StatusText.Text="جارٍ الترجمة...";}
        private void Window_OnKeyDown(object sender,KeyEventArgs e){if(e.Key==Key.Escape&&CancelRequested!=null) CancelRequested(this,EventArgs.Empty);}
    }
}
