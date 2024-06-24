namespace SFTPManager.Views.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            List<string> languages = new List<string> { "Ukrainian", "English" };
            languageComboBox.SelectionChanged += LanguageChange;
            languageComboBox.ItemsSource = languages;
            languageComboBox.SelectedItem = "English";
            List<string> styles = new List<string> { "Light", "Dark" };
            styleBox.SelectionChanged += ThemeChange;
            styleBox.ItemsSource = styles;
            styleBox.SelectedItem = "Dark";
        }

        private void LanguageChange(object sender, SelectionChangedEventArgs e)
        {
            string language = languageComboBox.SelectedItem as string;

            var languageUri = new Uri("..\\Resources/" + language + ".xaml", UriKind.Relative);
            ResourceDictionary languageDictionary = Application.LoadComponent(languageUri) as ResourceDictionary;

            ReplaceResourceDictionary("LanguageDictionary", languageDictionary);
        }

        private void ThemeChange(object sender, SelectionChangedEventArgs e)
        {
            string style = styleBox.SelectedItem as string;

            var themeUri = new Uri("..\\Themes/" + style + ".xaml", UriKind.Relative);
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
