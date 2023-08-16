// Options
// Learning UWP
// Common binding converter
//
// 2018-09-18   PV

using System;
using Windows.UI.Xaml.Data;

namespace OptionsNS;

public class NullableBooleanToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool?)
            return (bool)value;
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) 
        => value is bool v && v;
        //=> value is bool v ? v : false;
}
