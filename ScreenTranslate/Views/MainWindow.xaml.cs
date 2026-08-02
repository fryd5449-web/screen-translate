using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Forms=System.Windows.Forms;
using ScreenTranslate.Models;
using ScreenTranslate.Services;
using ScreenTranslate.ViewModels;
namespace ScreenTranslate.Views
{
 public partial class MainWindow:Window
 {
  readonly ISettingsService _settingsService; readonly IScreenCaptureService _screenCaptureService; readonly IOcrService _ocrService; readonly ITranslationService _translationService; readonly IUpdateService _updateService; readonly AppSettings _settings; readonly MainViewModel _viewModel;
  TranslationWindow _translationWindow; bool _busy,_allowClose;
  public event EventHandler<string> BackgroundNoticeRequested;
  public event EventHandler<bool> SelectionActivityChanged;
  public MainWindow(ISettingsService settingsService,IScreenCaptureService screenCaptureService,IOcrService ocrService,ITranslationService translationService,IUpdateService updateService)
  { _settingsService=settingsService;_screenCaptureService=screenCaptureService;_ocrService=ocrService;_translationService=translationService;_updateService=updateService;_settings=_settingsService.Load();_viewModel=new MainViewModel{StartWithWindows=_settings.StartWithWindows,KeepTranslationOnTop=_settings.KeepTranslationOnTop};DataContext=_viewModel;InitializeComponent();ApplySavedSourceLanguage(_settings.SourceLanguageCode);_viewModel.PropertyChanged+=ViewModel_OnPropertyChanged; }
  public async void BeginSelectionFromExternalTrigger(){await BeginSelectionAsync();} public void AllowCloseForExit(){_allowClose=true;}
  public void OpenSettingsWindow(){ShowSettingsWindow();}
  public IEnumerable<LanguageOption> SourceLanguages { get { return _viewModel.SourceLanguages; } }
  public string SelectedSourceLanguageCode { get { return _viewModel.SelectedSourceLanguage!=null?_viewModel.SelectedSourceLanguage.Code:"auto"; } }
  public void SetSourceLanguage(string code){ApplySavedSourceLanguage(code);SaveSourceLanguage();}
  private async void SelectFromScreenButton_OnClick(object s,RoutedEventArgs e){await BeginSelectionAsync();}
  private async Task BeginSelectionAsync()
  {
   if(_busy)return;_busy=true;NotifySelectionActivity(true);SelectFromScreenButton.IsEnabled=false;bool restore=IsVisible;Hide();ScreenCaptureResult capture=null;ProcessingWindow progress=null;bool canceled=false;
   try
   {
    capture=await _screenCaptureService.CaptureAreaAsync(); if(capture==null||capture.IsCancelled){_viewModel.ProgramStatus="تم إلغاء التحديد";return;}
    progress=new ProcessingWindow();progress.CancelRequested+=(s,e)=>canceled=true;PositionWindow(capture.PixelBounds,progress);progress.Show();
    _viewModel.ProgramStatus="جارٍ قراءة النص...";var source=_viewModel.SelectedSourceLanguage!=null?_viewModel.SelectedSourceLanguage.Code:"auto";var ocr=await _ocrService.ExtractTextAsync(capture.CapturedBitmap,source);
    if(canceled){_viewModel.ProgramStatus="تم إلغاء العملية";return;}
    if(ocr==null||string.IsNullOrWhiteSpace(ocr.Text)){_viewModel.ProgramStatus="لم يتم العثور على نص في المنطقة المحددة.";restore=false;NotifyInBackground(_viewModel.ProgramStatus);return;}
    progress.SetTranslating();_viewModel.ProgramStatus="جارٍ الترجمة...";var translated=await _translationService.TranslateToArabicAsync(ocr.Text,ocr.DetectedLanguageCode);
    if(string.IsNullOrWhiteSpace(translated))throw new TranslationException("لم تُرجع خدمة الترجمة نتيجة.");
    progress.Close();progress=null;EnsureTranslationWindow();_translationWindow.UpdateContent(ocr.Text,translated,ocr.DetectedLanguageNameArabic,_viewModel.SelectedSourceLanguage!=null?_viewModel.SelectedSourceLanguage.DisplayName:"كشف تلقائي","العربية");PositionWindow(capture.PixelBounds,_translationWindow);_translationWindow.Show();_translationWindow.Activate();_viewModel.ProgramStatus="جاهز";restore=false;
   }
   catch(TranslationException ex){ErrorLogger.Log("translation",ex);_viewModel.ProgramStatus=ex.Message;restore=false;NotifyInBackground(ex.Message);}
   catch(Exception ex){ErrorLogger.Log("capture-ocr",ex);_viewModel.ProgramStatus="تعذر إكمال قراءة النص.";restore=false;NotifyInBackground("تعذر إكمال قراءة النص. حاول مرة أخرى.");}
   finally{if(progress!=null)progress.Close();if(capture!=null)capture.Dispose();_busy=false;SelectFromScreenButton.IsEnabled=true;NotifySelectionActivity(false);if(restore){Show();WindowState=WindowState.Normal;Activate();}}
  }
  void EnsureTranslationWindow(){if(_translationWindow!=null)return;_translationWindow=new TranslationWindow();_translationWindow.RetryRequested+=TranslationWindowOnRetryRequested;_translationWindow.Closed+=(s,e)=>_translationWindow=null;}
  async void TranslationWindowOnRetryRequested(object s,EventArgs e){if(_translationWindow!=null)_translationWindow.Hide();await BeginSelectionAsync();}
  private void SettingsButton_OnClick(object s,RoutedEventArgs e){ShowSettingsWindow();}
  private void ShowSettingsWindow(){new AboutUpdatesWindow(_updateService){Owner=this}.ShowDialog();}
  private void MinimizeToTrayButton_OnClick(object s,RoutedEventArgs e){Hide();}
  private void Header_OnMouseLeftButtonDown(object s,MouseButtonEventArgs e){if(e.ChangedButton==MouseButton.Left)DragMove();}
  private void KeepOnTopCheckBox_OnChecked(object s,RoutedEventArgs e){_settings.KeepTranslationOnTop=_viewModel.KeepTranslationOnTop;_settingsService.Save(_settings);if(_translationWindow!=null)_translationWindow.Topmost=_viewModel.KeepTranslationOnTop;}
  private void StartWithWindowsCheckBox_OnChecked(object s,RoutedEventArgs e){_settings.StartWithWindows=_viewModel.StartWithWindows;_settingsService.Save(_settings);}
  protected override void OnClosing(CancelEventArgs e){if(!_allowClose){e.Cancel=true;Hide();return;}base.OnClosing(e);}
  protected override void OnStateChanged(EventArgs e){base.OnStateChanged(e);if(WindowState==WindowState.Minimized)Hide();}
  private void NotifyInBackground(string message){if(BackgroundNoticeRequested!=null)BackgroundNoticeRequested(this,message);}
  private void NotifySelectionActivity(bool active){if(SelectionActivityChanged!=null)SelectionActivityChanged(this,active);}
  private void ViewModel_OnPropertyChanged(object sender,PropertyChangedEventArgs e){if(e.PropertyName=="SelectedSourceLanguage")SaveSourceLanguage();}
  private void SaveSourceLanguage(){_settings.SourceLanguageCode=SelectedSourceLanguageCode;_settingsService.Save(_settings);}
  void ApplySavedSourceLanguage(string code){foreach(var l in _viewModel.SourceLanguages)if(string.Equals(l.Code,code,StringComparison.OrdinalIgnoreCase)){_viewModel.SelectedSourceLanguage=l;return;}_viewModel.SelectedSourceLanguage=_viewModel.SourceLanguages[0];}
  static void PositionWindow(Rectangle bounds,Window window){var area=Forms.Screen.FromRectangle(bounds).WorkingArea;const int margin=12;var x=bounds.Right+margin;var y=bounds.Top;if(x+window.Width>area.Right)x=bounds.Left-(int)window.Width-margin;if(x<area.Left)x=area.Left+margin;if(y+window.Height>area.Bottom)y=area.Bottom-(int)window.Height-margin;if(y<area.Top)y=area.Top+margin;var dpi=VisualTreeHelper.GetDpi(window);window.Left=x/dpi.DpiScaleX;window.Top=y/dpi.DpiScaleY;}
 }
}
