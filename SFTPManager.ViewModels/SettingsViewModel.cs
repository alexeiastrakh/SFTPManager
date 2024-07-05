namespace SFTPManager.ViewModels
{
    using System.Windows;
    using CommunityToolkit.Mvvm.ComponentModel;

    public class SettingsViewModel : ObservableObject
    {
        private string selectedLanguage;
        private string selectedStyle;

        public SettingsViewModel()
        {
            Languages = new List<string> { "Українська", "English" };
            Styles = new List<string> { "Light", "Dark" };
            SelectedLanguage = "English";
            SelectedStyle = "Dark";
        }

        public List<string> Languages { get; }

        public List<string> Styles { get; }

        public string SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (SetProperty(ref selectedLanguage, value))
                {
                    ChangeLanguage();
                }
            }
        }

        public string SelectedStyle
        {
            get => selectedStyle;
            set
            {
                if (SetProperty(ref selectedStyle, value))
                {
                    ChangeStyle();
                }
            }
        }

        private void ChangeLanguage()
        {
            var languageUri = new Uri("/SFTPManager.Resources;component/Languages/" + SelectedLanguage + ".xaml", UriKind.Relative);
            ResourceDictionary languageDictionary = Application.LoadComponent(languageUri) as ResourceDictionary;
            ReplaceResourceDictionary("LanguageDictionary", languageDictionary);
        }


        private void ChangeStyle()
        {
            var themeUri = new Uri("/SFTPManager.Resources;component/Themes/" + SelectedStyle + ".xaml", UriKind.Relative);
            ResourceDictionary themeDictionary = Application.LoadComponent(themeUri) as ResourceDictionary;
            ReplaceResourceDictionary("ThemeDictionary", themeDictionary);
        }

        private void ReplaceResourceDictionary(string dictionaryKey, ResourceDictionary newDictionary)
        {
            var existingDictionary = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Contains(dictionaryKey));

            if (existingDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(existingDictionary);
            }

            newDictionary.Add(dictionaryKey, null);
            Application.Current.Resources.MergedDictionaries.Add(newDictionary);
        }
    }
}
