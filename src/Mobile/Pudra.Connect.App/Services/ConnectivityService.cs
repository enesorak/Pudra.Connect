using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.Services;

public class ConnectivityService : IConnectivityService
{
    private readonly IApiService _apiService; // ApiService'in içindeki HttpClient'ı kullanabiliriz

    public ConnectivityService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<bool> IsApiReachableAsync()
    {
        // 1. Önce cihazın genel bir internet bağlantısı var mı diye kontrol et.
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            return false;
        }

        try
        {
            // 2. API'nin kendisine çok kısa bir timeout ile ping atmayı dene.
            // Bu metodu ApiService'e ekleyelim.
            return await _apiService.PingApiAsync();
        }
        catch
        {
            return false;
        }
    }
}