using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

   /* public ApiService()
    {
        var handler = new HttpClientHandler();
#if DEBUG
        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
#endif

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(GetBaseUrlForPlatform()),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }*/

    public ApiService(IHttpClientFactory httpClientFactory)
    {
        
#if DEBUG
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
#endif
        
        
        _httpClient = httpClientFactory.CreateClient("PudraApi");
    }

    public async Task<T?> GetAsync<T>(string requestUri) where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var response = await SendAuthenticatedRequestAsync(request);
        return await HandleResponseAsync<T>(response);
    }

    public async Task<T?> PostAsync<T>(string requestUri, object? payload = null) where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        if (payload != null)
        {
            request.Content = JsonContent.Create(payload);
        }

        // Login ve Register token gerektirmez
        var response = (requestUri.Contains("api/auth/login") || requestUri.Contains("api/auth/public-register"))
            ? await _httpClient.SendAsync(request)
            : await SendAuthenticatedRequestAsync(request);

        return await HandleResponseAsync<T>(response);
    }

    public async Task<HttpResponseMessage> PutAsync(string requestUri, object payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = JsonContent.Create(payload)
        };
        return await SendAuthenticatedRequestAsync(request);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        return await SendAuthenticatedRequestAsync(request);
    }

    public async Task<bool> PingApiAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await _httpClient.GetAsync("/api/test/ping", cts.Token);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // --- ÖZEL YARDIMCI METOTLAR ---

    private async Task<HttpResponseMessage> SendAuthenticatedRequestAsync(HttpRequestMessage request)
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (string.IsNullOrEmpty(token))
        {
            await HandleUnauthorizedAsync();
            throw new UnauthorizedAccessException("No authentication token found.");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync();
            throw new UnauthorizedAccessException("Session has expired.");
        }

        return response;
    }

    private async Task<T?> HandleResponseAsync<T>(HttpResponseMessage response) where T : class
    {
        if (response.IsSuccessStatusCode)
        {
            // Boş içerik durumunu kontrol et
            if (response.Content.Headers.ContentLength == 0) return default(T);
            return await response.Content.ReadFromJsonAsync<T>();
        }

        Debug.WriteLine($"[ApiService] API Error: {(int)response.StatusCode} on {response.RequestMessage?.RequestUri}");
        return null;
    }

    private async Task HandleUnauthorizedAsync()
    {
        SecureStorage.Default.RemoveAll();
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.DisplayAlert("Oturum Süresi Doldu",
                "Güvenlik nedeniyle oturumunuz sonlandırıldı. Lütfen tekrar giriş yapın.", "Tamam");
            await Shell.Current.GoToAsync("//LoginPage");
        });
    }

    private string GetBaseUrlForPlatform()
    {
        // --- CANLI (RELEASE) MODU ---
        // Uygulamayı yayınlamak için derlediğimizde, her zaman bu adresi kullanır.
        return "https://app.pudraa.de";
    }
}