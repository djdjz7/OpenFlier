using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenFlier.Controls;

public class AttachedSimpleProperties:DependencyObject
{


    public static double GetDouble(DependencyObject obj)
    {
        return (double)obj.GetValue(DoubleProperty);
    }

    public static void SetDouble(DependencyObject obj, double value)
    {
        obj.SetValue(DoubleProperty, value);
    }

    // Using a DependencyProperty as the backing store for Double.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DoubleProperty =
        DependencyProperty.RegisterAttached("Double", typeof(double), typeof(AttachedSimpleProperties), new PropertyMetadata(0.0));


}
