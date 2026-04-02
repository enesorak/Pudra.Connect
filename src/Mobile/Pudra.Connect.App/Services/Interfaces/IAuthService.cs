 
using System.Threading.Tasks;
using Pudra.Connect.App.Models;
using Pudra.Connect.App.Models.Auth;

namespace Pudra.Connect.App.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string username, string password);
    Task<PublicRegisterResponseDto?> RegisterAsync(PublicRegisterRequestDto request);

}