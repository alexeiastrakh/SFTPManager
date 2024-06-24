namespace SFTPManager.Services
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class EmptyCollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                bool inverse = parameter != null && parameter.ToString() == "True";
                if (inverse)
                {
                    return count > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
