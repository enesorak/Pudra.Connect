using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pudra.Connect.Application.Dtos.Auth;
using Pudra.Connect.Application.Interfaces;
using Pudra.Connect.Domain.Entities;
using Pudra.Connect.Domain.Enums;
using Pudra.Connect.Infrastructure.Persistence;

namespace Pudra.Connect.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private const int MaxTrialScans = 50;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        // Kullanıcı yoksa veya şifre yanlışsa hata fırlat
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        // SADECE Rejected kullanıcılar giriş yapamaz
        if (user.Status == UserStatus.Rejected)
        {
            throw new UnauthorizedAccessException("Your account has been rejected. Please contact support.");
        }

        // Pending kullanıcılar da giriş yapabilir (50 scan hakkıyla)
        var token = GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Token = token,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            RemainingScans = user.Status == UserStatus.Pending ? MaxTrialScans - user.ScanCount : -1,
            ProfileImageUrl = user.ProfileImageUrl // YENİ

        };
    }

    public async Task<bool> RegisterAsync(string username, string fullName, string password, string role)
    {
        // Kullanıcı adı zaten var mı kontrol et
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            return false;
        }
        
        // Enum'a çevirmeyi dene
        if (!Enum.TryParse<Role>(role, true, out var userRole))
        {
            throw new ArgumentException("Invalid role specified.", nameof(role));
        }

        var user = new User
        {
            Username = username,
            FullName = fullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = userRole,
            Status = UserStatus.Approved // Admin tarafından oluşturulan kullanıcılar direkt onaylı
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return true;
    }

    // Public Register - Herkes kayıt olabilir, 50 scan hakkıyla başlar
    public async Task<PublicRegisterResponseDto> PublicRegisterAsync(PublicRegisterRequestDto request)
    {
        // Email zaten kayıtlı mı?
        if (await _context.Users.AnyAsync(u => u.Username == request.Email))
        {
            return new PublicRegisterResponseDto
            {
                Success = false,
                Message = "This email is already registered."
            };
        }

        var user = new User
        {
            Username = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.Seller,
            Status = UserStatus.Pending,
            ScanCount = 0
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new PublicRegisterResponseDto
        {
            Success = true,
            Message = "Registration successful! You can now login and try the app with 50 free scans."
        };
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("status", user.Status.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24 * 180),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}