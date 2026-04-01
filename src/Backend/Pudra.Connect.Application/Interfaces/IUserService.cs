using Pudra.Connect.Application.Dtos.Users;

namespace Pudra.Connect.Application.Interfaces;

public interface IUserService
{
    // User CRUD
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<IEnumerable<UserDto>> GetAllUsersAsync(string currentUserId);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> CreateUserAsync(CreateUserRequestDto userDto);
    Task<bool> UpdateUserAsync(string userId, UpdateUserRequestDto userDto);
    Task<bool> DeleteUserAsync(string userId);

    // Profile
    Task<bool> UpdateMyProfileAsync(string userId, string newFullName);
    Task<bool> ChangeMyPasswordAsync(string userId, string oldPassword, string newPassword);
    Task<bool> UpdateProfileImageAsync(string userId, string imageBase64);

    // Pending Users
    Task<IEnumerable<UserDto>> GetPendingUsersAsync();
    Task<bool> ApproveUserAsync(string userId);
    Task<bool> RejectUserAsync(string userId);

    // Scan Limit
    Task<bool> CanUserScanAsync(string userId);
    Task IncrementScanCountAsync(string userId);
}