// În Restaurant.Converters.BoolToVisibilityConverter.cs
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
            else if (value is int intVal) // Adaugă această verificare
            {
                bValue = (intVal != 0); // Consideră 0 ca fals, orice altceva ca adevărat
            }
            // Poți adăuga și alte tipuri dacă este necesar

            // Logica de inversare (dacă parametrul este "Invert" sau similar)
            bool invert = parameter as string == "Invert";
            if (invert)
            {
                bValue = !bValue;
            }

            return bValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // De obicei, nu este necesar pentru Visibility
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