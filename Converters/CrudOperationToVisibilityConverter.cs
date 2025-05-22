using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Restaurant.ViewModels; 

namespace Restaurant.Converters
{
    public class CrudOperationToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CrudPopupOperationType currentOperation && parameter is string targetOperationsString)
            {
                var targetOperations = targetOperationsString.Split('|');
                foreach (var targetOpString in targetOperations)
                {
                    if (Enum.TryParse<CrudPopupOperationType>(targetOpString, true, out var targetOperation))
                    {
                        if (currentOperation == targetOperation)
                            return Visibility.Visible;
                    }
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