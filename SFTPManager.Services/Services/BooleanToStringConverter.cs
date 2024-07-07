namespace SFTPManager.Core.Services
{
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null || value == null)
                return false;

            return value.ToString() == parameterString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null || value == null)
                return null;

            return (bool)value ? parameterString : null;
        }
    }
}
