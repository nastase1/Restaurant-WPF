using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Restaurant.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;

            if (value is bool boolVal)
            {
                bValue = boolVal;
            }
            else if (value is int intVal) 
            {
                bValue = (intVal != 0); 
            }

            bool invert = parameter as string == "Invert";
            if (invert)
            {
                bValue = !bValue;
            }

            return bValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool invert = parameter as string == "Invert";
                bool bValue = (visibility == Visibility.Visible);
                return invert ? !bValue : bValue;
            }
            return false;
        }
    }
}