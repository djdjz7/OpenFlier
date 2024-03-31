using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenFlier.Desktop.Converters
{

    public class FalseToStrikethroughConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is true)
                return TextDecorationCollectionConverter.ConvertFromString("Strikethrough");
            return new TextDecorationCollection();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
