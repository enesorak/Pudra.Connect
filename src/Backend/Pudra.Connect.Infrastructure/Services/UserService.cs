using Microsoft.EntityFrameworkCore;
using Pudra.Connect.Application.Dtos.Users;
using Pudra.Connect.Application.Interfaces;
using Pudra.Connect.Domain.Entities;
using Pudra.Connect.Domain.Enums;
using Pudra.Connect.Infrastructure.Persistence;

namespace Pudra.Connect.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private const int MaxTrialScans = 50;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role.ToString(),
                Status = u.Status.ToString(),
                ProfileImageUrl = u.ProfileImageUrl
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(string currentUserId)
    {
        return await _context.Users
            .Where(u => u.Id != currentUserId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role.ToString(),
                Status = u.Status.ToString(),
                ProfileImageUrl = u.ProfileImageUrl
            })
            .ToListAsync();
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            ProfileImageUrl = user.ProfileImageUrl
        };
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserRequestDto userDto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
        {
            return null;
        }

        if (!Enum.TryParse<Role>(userDto.Role, true, out var userRole))
        {
            return null;
        }

        var user = new User
        {
            Username = userDto.Username,
            FullName = userDto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            Role = userRole,
            Status = UserStatus.Approved
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            ProfileImageUrl = user.ProfileImageUrl
        };
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequestDto userDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (!Enum.TryParse<Role>(userDto.Role, true, out var userRole))
        {
            return false;
        }

        user.FullName = userDto.FullName;
        user.Role = userRole;

        if (!string.IsNullOrWhiteSpace(userDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateMyProfileAsync(string userId, string newFullName)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.FullName = newFullName;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeMyPasswordAsync(string userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProfileImageAsync(string userId, string imageBase64)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (!string.IsNullOrEmpty(imageBase64) && !imageBase64.StartsWith("data:image/"))
        {
            return false;
        }

        user.ProfileImageUrl = imageBase64;
        await _context.SaveChangesAsync();
        return true;
    }

    // Pending Users Yönetimi
    public async Task<IEnumerable<UserDto>> GetPendingUsersAsync()
    {
        return await _context.Users
            .Where(u => u.Status == UserStatus.Pending)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role.ToString(),
                Status = u.Status.ToString(),
                ProfileImageUrl = u.ProfileImageUrl
            })
            .ToListAsync();
    }

    public async Task<bool> ApproveUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Status = UserStatus.Approved;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Status = UserStatus.Rejected;
        await _context.SaveChangesAsync();
        return true;
    }

    // Scan Limit Kontrolü
    public async Task<bool> CanUserScanAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (user.Status == UserStatus.Approved) return true;

        if (user.Status == UserStatus.Pending && user.ScanCount < MaxTrialScans) return true;

        return false;
    }

    public async Task IncrementScanCountAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        user.ScanCount++;
        await _context.SaveChangesAsync();
    }
}