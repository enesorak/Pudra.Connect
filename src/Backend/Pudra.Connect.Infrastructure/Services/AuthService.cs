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

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            // Kullanıcı yoksa veya şifre yanlışsa hata fırlat.
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Her şey doğruysa, JWT üret ve geri dön.
            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Role = user.Role.ToString()
            };
        }

        public async Task<bool> RegisterAsync(string username, string fullName, string password, string role)
        {
            // Kullanıcı adı zaten var mı kontrol et.
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                return false; // veya bir exception fırlatılabilir.
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
                // Şifreyi hash'le ve kaydet.
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = userRole
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Subject = User ID
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Her token için benzersiz ID
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24*180), // Token geçerlilik süresi
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }