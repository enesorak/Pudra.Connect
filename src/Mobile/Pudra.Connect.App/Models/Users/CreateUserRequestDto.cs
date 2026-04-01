namespace Pudra.Connect.App.Models.Users;

public class CreateUserRequestDto
{
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = "Seller"; // Varsayılan olarak Satıcı
}