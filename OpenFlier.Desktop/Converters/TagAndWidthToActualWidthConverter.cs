using System;
using System.Globalization;
using System.Windows.Data;

namespace OpenFlier.Desktop.Converters
{
    public class TagAndWidthToActualWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0] * (double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
