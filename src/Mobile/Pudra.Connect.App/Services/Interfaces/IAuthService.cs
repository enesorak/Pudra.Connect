using Pudra.Connect.App.Models;

namespace Pudra.Connect.App.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string username, string password);
}