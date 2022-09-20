using System;
using System.Globalization;
using System.Windows.Data;

namespace StringMaster.Converters;

[ValueConversion(typeof(bool), typeof(string))]
public class BooleanYesNoConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var boolValue = (bool)value;
        return boolValue ? "Yes" : "No";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}