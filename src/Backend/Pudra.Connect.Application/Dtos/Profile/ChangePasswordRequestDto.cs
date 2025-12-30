namespace Pudra.Connect.Application.Dtos.Profile;

public class ChangePasswordRequestDto
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}