using Pudra.Connect.App.Models.Users;

namespace Pudra.Connect.App.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>?> GetUsersAsync();
    Task<UserDto?> CreateUserAsync(CreateUserRequestDto newUser);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> UpdateUserAsync(string userId, UpdateUserRequestDto userDto);
}