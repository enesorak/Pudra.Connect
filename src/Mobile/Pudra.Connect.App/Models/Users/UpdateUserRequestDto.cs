namespace Pudra.Connect.App.Models.Users;

/// <summary>
/// Bir kullanıcının bilgilerini güncellemek için API'ye gönderilecek verileri temsil eder.
/// </summary>
public class UpdateUserRequestDto
{
    /// <summary>
    /// Kullanıcının yeni tam adı.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Kullanıcının yeni rolü ("Admin" veya "Seller").
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Bu alan sadece şifre değiştirilmek istendiğinde doldurulur.
    /// Eğer boş veya null gönderilirse, backend şifreyi değiştirmez.
    /// </summary>
    public string? Password { get; set; }
}