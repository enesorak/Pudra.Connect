using Pudra.Connect.App.Models.Profile;

namespace Pudra.Connect.App.Services.Interfaces;

public interface IProfileService
{
    Task<bool> UpdateMyProfileAsync(UpdateProfileRequestDto profileDto);
    Task<bool> ChangeMyPasswordAsync(ChangePasswordRequestDto passwordDto);
}