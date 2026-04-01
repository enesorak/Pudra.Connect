using System.Globalization;

namespace Pudra.Connect.App.Converters;

/// <summary>
/// Bir bool değerini iki farklı renkten birine dönüştürür.
/// Parametre olarak "TrueRengi,FalseRengi" formatında bir string bekler. Örn: "Yellow,White".
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Gelen değerin bool olup olmadığını kontrol et
        if (value is not bool isToggled)
            return Colors.Gray; // Varsayılan renk

        // Parametre olarak verilen renkleri al
        var colors = parameter?.ToString()?.Split(',') ?? new string[] { "Black", "Gray" };
        if (colors.Length < 2)
            return Colors.Gray;

        // Değere göre doğru rengi seç ve dönüştür
        string colorString = isToggled ? colors[0] : colors[1];
        return Color.FromArgb(colorString);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Bu yönde bir dönüşüme ihtiyacımız yok.
        throw new NotImplementedException();
    }
}