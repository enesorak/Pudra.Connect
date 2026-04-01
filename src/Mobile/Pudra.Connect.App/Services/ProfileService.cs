using Pudra.Connect.App.Models.Profile;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.Services;

public class ProfileService : IProfileService
{
    private readonly IApiService _apiService;

    public ProfileService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<bool> UpdateMyProfileAsync(UpdateProfileRequestDto profileDto)
    {
        var response = await _apiService.PutAsync("/api/profile", profileDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ChangeMyPasswordAsync(ChangePasswordRequestDto passwordDto)
    {
        var response = await _apiService.PutAsync("/api/profile/change-password", passwordDto);
        return response.IsSuccessStatusCode;
    }
}