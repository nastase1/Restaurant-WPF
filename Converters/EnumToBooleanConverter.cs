// În Restaurant.Converters.EnumToBooleanConverter.cs
using System;
using System.Globalization;
using System.Windows; // Necesar pentru DependencyProperty.UnsetValue
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
                return DependencyProperty.UnsetValue; // Folosește UnsetValue pentru a indica eșecul conversiei

            bool boolValue;
            if (value is bool)
            {
                boolValue = (bool)value;
            }
            else if (value is bool?) // Gestionează și bool?
            {
                boolValue = ((bool?)value) ?? false;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }

            if (!boolValue)
            {
                // Pentru RadioButton, deselectarea unuia nu selectează automat altul prin ConvertBack dacă toate sunt false.
                // Adesea, ConvertBack nu este critic pentru RadioButton-uri legate la un enum,
                // deoarece selecția unuia declanșează schimbarea valorii enum-ului în ViewModel,
                // care apoi actualizează toate RadioButton-urile prin Convert.
                // Dacă totuși ai nevoie să gestionezi un caz specific la "false",
                // poți returna DependencyProperty.UnsetValue sau valoarea curentă a enum-ului.
                // Pentru simplitate aici, vom returna parametrul (valoarea enum-ului) dacă e true.
                return DependencyProperty.UnsetValue;
            }

            try
            {
                // Asigură-te că targetType este tipul enum-ului
                return Enum.Parse(targetType, parameter.ToString(), true); // true pentru ignore case
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}