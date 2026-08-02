using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScreenTranslate.Models;
namespace ScreenTranslate.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _programStatus; private LanguageOption _selectedSourceLanguage; private bool _startWithWindows; private bool _keepTranslationOnTop;
        public MainViewModel()
        {
            SourceLanguages = new ObservableCollection<LanguageOption>
            {
                new LanguageOption { Code="auto", DisplayName="كشف تلقائي" }, new LanguageOption { Code="en", DisplayName="الإنجليزية" },
                new LanguageOption { Code="fr", DisplayName="الفرنسية" }, new LanguageOption { Code="de", DisplayName="الألمانية" },
                new LanguageOption { Code="es", DisplayName="الإسبانية" }, new LanguageOption { Code="ar", DisplayName="العربية" }
            };
            SelectedSourceLanguage = SourceLanguages[0]; ProgramStatus = "جاهز";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<LanguageOption> SourceLanguages { get; private set; }
        public string ProgramStatus { get { return _programStatus; } set { if (_programStatus == value) return; _programStatus=value; OnPropertyChanged(); } }
        public LanguageOption SelectedSourceLanguage { get { return _selectedSourceLanguage; } set { if (_selectedSourceLanguage == value) return; _selectedSourceLanguage=value; OnPropertyChanged(); } }
        public bool StartWithWindows { get { return _startWithWindows; } set { if (_startWithWindows == value) return; _startWithWindows=value; OnPropertyChanged(); } }
        public bool KeepTranslationOnTop { get { return _keepTranslationOnTop; } set { if (_keepTranslationOnTop == value) return; _keepTranslationOnTop=value; OnPropertyChanged(); } }
        private void OnPropertyChanged([CallerMemberName] string name=null) { if(PropertyChanged!=null)PropertyChanged(this,new PropertyChangedEventArgs(name)); }
    }
}
