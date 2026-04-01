namespace Pudra.Connect.Application.Dtos.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RemainingScans { get; set; } = -1; // -1 = sınırsız, 0-50 = kalan hak
    
    public string? ProfileImageUrl { get; set; } // YENİ

}