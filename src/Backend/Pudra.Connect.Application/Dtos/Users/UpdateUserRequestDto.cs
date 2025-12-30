namespace Pudra.Connect.Application.Dtos.Users;

public class UpdateUserRequestDto
{
    public string FullName { get; set; }
    public string Role { get; set; }
    public string? Password { get; set; } // Null olabilir, şifre değiştirilmeyecekse boş gönderilir.
}