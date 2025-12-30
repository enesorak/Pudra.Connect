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
                Role = u.Role.ToString()
            })
            .ToListAsync();
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserRequestDto userDto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
        {
            // Bu kullanıcı adı zaten var.
            return null;
        }

        if (!Enum.TryParse<Role>(userDto.Role, true, out var userRole))
        {
            // Geçersiz rol
            return null;
        }

        var user = new User
        {
            Username = userDto.Username,
            FullName = userDto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            Role = userRole
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new UserDto { Id = user.Id, Username = user.Username, FullName = user.FullName, Role = user.Role.ToString() };
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
            return false; // Geçersiz rol
        }

        user.FullName = userDto.FullName;
        user.Role = userRole;

        // Sadece yeni bir şifre gönderildiyse güncelle
        if (!string.IsNullOrWhiteSpace(userDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
    
    
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(string currentUserId)
    {
        return await _context.Users
            .Where(u => u.Id != currentUserId) // <-- KENDİSİNİ HARİÇ TUTAN KRİTİK FİLTRE
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role.ToString()
            })
            .ToListAsync();
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
        // Kullanıcı yoksa veya eski şifre yanlışsa, başarısız ol.
        if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
}