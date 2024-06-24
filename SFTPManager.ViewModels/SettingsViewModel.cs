namespace SFTPManager.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using System.Windows.Input;

    public class SettingsViewModel : ObservableObject
    {
        private string _selectedLanguage;

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public ICommand ChangeLanguageCommand { get; }

        public SettingsViewModel()
        {
            ChangeLanguageCommand = new RelayCommand<string>(ChangeLanguage);
        }

        private void ChangeLanguage(string language)
        {
            SelectedLanguage = language;
        }
    }
}
