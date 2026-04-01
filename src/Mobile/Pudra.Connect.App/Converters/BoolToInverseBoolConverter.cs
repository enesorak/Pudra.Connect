using System.Globalization;

namespace Pudra.Connect.App.Converters;


public class BoolToInverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !(bool)(value ?? throw new ArgumentNullException(nameof(value)));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !(bool)(value ?? throw new ArgumentNullException(nameof(value)));
    }
}