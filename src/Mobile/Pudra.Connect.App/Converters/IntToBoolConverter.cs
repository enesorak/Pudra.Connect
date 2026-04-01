using System.Globalization;

namespace Pudra.Connect.App.Converters;

/// <summary>
/// Bir integer değeri bool değerine dönüştürür.
/// Gelen sayı 0'dan büyükse 'true', değilse 'false' döndürür.
/// </summary>
public class IntToBoolConverter : IValueConverter
{
    /// <summary>
    /// Değeri integer'dan bool'a dönüştürür.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Gelen değerin bir integer olup olmadığını kontrol et
        if (value is int intValue)
        {
            // Eğer sayı 0'dan büyükse, true (görünür) döndür.
            return intValue > 0;
        }
            
        // Beklenmedik bir tip gelirse veya null ise, false (görünmez) döndür.
        return false;
    }

    /// <summary>
    /// Bu yönde bir dönüşüme ihtiyacımız olmadığı için bu metot boş kalacak.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}