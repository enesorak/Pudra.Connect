namespace Pudra.Connect.App.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RemainingScans { get; set; } = -1;
    public string? ProfileImageUrl { get; set; }
}