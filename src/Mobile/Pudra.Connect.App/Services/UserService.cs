using Pudra.Connect.App.Models.Users;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.Services;

public class UserService : IUserService
{
    private readonly IApiService _apiService; // Düşük seviye API servisine bağımlı

    public UserService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public Task<List<UserDto>?> GetUsersAsync()
    {
        return _apiService.GetAsync<List<UserDto>>("/api/users");
    }

    public Task<UserDto?> CreateUserAsync(CreateUserRequestDto newUser)
    {
        return _apiService.PostAsync<UserDto>("/api/users", newUser);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var response = await _apiService.DeleteAsync($"/api/users/{userId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequestDto userDto)
    {
        var response = await _apiService.PutAsync($"/api/users/{userId}", userDto);
        return response.IsSuccessStatusCode;
    }
}