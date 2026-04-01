namespace Pudra.Connect.App.Models.Profile;

public class ChangePasswordRequestDto
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}