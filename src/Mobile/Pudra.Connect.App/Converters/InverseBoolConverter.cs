using System.Globalization;

namespace Pudra.Connect.App.Converters;

/// <summary>
/// Gelen bir bool değerini tam tersine çevirir (true -> false, false -> true).
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Geri dönüşüm genellikle gerekli değildir.
        throw new NotImplementedException();
    }
}