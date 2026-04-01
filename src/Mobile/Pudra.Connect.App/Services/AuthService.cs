using Pudra.Connect.App.Models;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.Services;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    public AuthService(IApiService apiService) { _apiService = apiService; }

    public Task<LoginResponse?> LoginAsync(string username, string password)
    {
        // Login isteği token gerektirmediği için, IApiService'de bu isteği
        // özel olarak ele alacak bir metot olmalı (daha önceki gibi).
        // Veya PostAsync'i public yapabiliriz. Şimdilik bu şekilde bırakıyorum.
        // Bu, ApiService'in iç mantığıdır.
        return _apiService.PostAsync<LoginResponse>("/api/auth/login", new { username, password });
    }
}