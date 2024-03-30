using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace OpenFlier.Desktop.Converters
{
    public class StringListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.Join(", ", (IList<string>)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
