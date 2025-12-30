using Pudra.Connect.Application.Dtos.Users;

namespace Pudra.Connect.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> CreateUserAsync(CreateUserRequestDto userDto);
    Task<bool> DeleteUserAsync(string userId);
    
        Task<bool> UpdateUserAsync(string userId, UpdateUserRequestDto userDto);
        
        
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string currentUserId);
        
        
        Task<bool> ChangeMyPasswordAsync(string userId, string oldPassword, string newPassword);
        Task<bool> UpdateMyProfileAsync(string userId, string newFullName);


}