namespace Pudra.Connect.App.Models.Auth;

public class PublicRegisterRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    // yorum ekleyelim ki gitsin amk
}
