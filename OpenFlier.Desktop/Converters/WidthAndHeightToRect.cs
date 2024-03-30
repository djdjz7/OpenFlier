﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenFlier.Desktop.Converters
{
    public class WidthAndHeightToRect : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Rect(0, 0, (double)values[0], (double)values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] values = new object[2];
            Rect rect = (Rect)value;
            values[0] = rect.X;
            values[1] = rect.Y;
            return values;
        }
    }
}