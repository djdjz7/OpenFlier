﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenFlier.Controls.Converters;

public class ScrollBarMaximumToVisibilityConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value as double? == 0)
            return Visibility.Collapsed;
        else
            return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
