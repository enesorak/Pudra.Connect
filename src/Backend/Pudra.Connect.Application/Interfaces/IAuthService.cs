using Pudra.Connect.Application.Dtos.Auth;

namespace Pudra.Connect.Application.Interfaces;
public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<bool> RegisterAsync(string username, string fullName, string password, string role);
}