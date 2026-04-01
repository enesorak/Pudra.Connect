namespace Pudra.Connect.Application.Dtos.Auth;

public class PublicRegisterRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;  // Email = Username olarak kullanılacak
    public string Password { get; set; } = string.Empty;
}