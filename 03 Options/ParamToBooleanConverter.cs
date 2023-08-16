// Options
// Learning UWP
// Common binding converter
//
// 2018-09-25   PV

using System;
using Windows.UI.Xaml.Data;

namespace OptionsNS;

public class ParamToBooleanConverter : IValueConverter
{
    // Forcing ToString enables the use of the same converter for ints and enums
    public object Convert(object value, Type targetType, object parameter, string language)
        => value.ToString().Equals(parameter.ToString());

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value.Equals(true)) return parameter;
        // return Binding.DoNothing();
        //throw new ArgumentException();
        return false;
    }
}

