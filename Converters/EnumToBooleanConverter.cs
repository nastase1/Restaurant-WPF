using System;
using System.Globalization;
using System.Windows; 
using System.Windows.Data;

namespace Restaurant.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string enumValue = value.ToString();
            string targetValue = parameter.ToString();
            return enumValue.Equals(targetValue, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || targetType == null)
                return DependencyProperty.UnsetValue; 

            bool boolValue;
            if (value is bool)
            {
                boolValue = (bool)value;
            }
            else if (value is bool?) 
            {
                boolValue = ((bool?)value) ?? false;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }

            if (!boolValue)
            {
                return DependencyProperty.UnsetValue;
            }

            try
            {
                return Enum.Parse(targetType, parameter.ToString(), true); 
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}