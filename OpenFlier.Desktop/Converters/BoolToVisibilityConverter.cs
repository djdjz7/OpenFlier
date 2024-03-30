using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenFlier.Desktop.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool?)value == null || (bool?)value == false)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
                return true;
            return false;
        }
    }
}
