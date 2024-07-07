namespace SFTPManager.ViewModels
{
    using System.Globalization;
    using System.Resources;
    using System.Windows;
    using CommunityToolkit.Mvvm.ComponentModel;
    using SFTPManager.Resources;

    public class SettingsViewModel : ObservableObject
    {
        private string selectedLanguage;
        private string selectedStyle;

        public SettingsViewModel()
        {
            // Initialize with localized language and theme names
            InitializeLanguages();
            InitializeStyles();

            // Default selections
            SelectedLanguage = Resources_en.English; // Default language
            SelectedStyle = Resources_en.Dark; // Default theme
        }

        public List<string> Languages { get; private set; }

        public List<string> Styles { get; private set; }

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

        private void InitializeLanguages()
        {
            Languages = new List<string> { Resources_en.Ukrainian, Resources_en.English };
        }

        private void InitializeStyles()
        {
            Styles = new List<string> { Resources_en.Light, Resources_en.Dark };
        }

        private void ChangeLanguage()
        {
            string languageCode = SelectedLanguage == Resources_en.Ukrainian ? "uk-UA" : "en-US";
            var languageUri = new Uri("/SFTPManager.Resources;component/Languages/" + languageCode + ".xaml", UriKind.Relative);
            ResourceDictionary languageDictionary = Application.LoadComponent(languageUri) as ResourceDictionary;
            ReplaceResourceDictionary("LanguageDictionary", languageDictionary);

            InitializeLanguages();
        }

        private void ChangeStyle()
        {
            var themeUri = new Uri("/SFTPManager.Resources;component/Themes/" + SelectedStyle + ".xaml", UriKind.Relative);
            ResourceDictionary themeDictionary = Application.LoadComponent(themeUri) as ResourceDictionary;
            ReplaceResourceDictionary("ThemeDictionary", themeDictionary);

            InitializeStyles();
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
