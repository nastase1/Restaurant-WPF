using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Restaurant.Converters
{
    public class StringNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = (parameter as string) == "Invert";
            bool isNullOrEmpty = string.IsNullOrEmpty(value as string);
            bool shouldShow = invert ? !isNullOrEmpty : isNullOrEmpty;
            return shouldShow ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
