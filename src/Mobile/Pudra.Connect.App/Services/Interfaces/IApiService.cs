
namespace Pudra.Connect.App.Services.Interfaces;

public interface IApiService
{
    Task<T?> GetAsync<T>(string requestUri) where T : class;
    Task<T?> PostAsync<T>(string requestUri, object payload) where T : class;
    Task<HttpResponseMessage> PutAsync(string requestUri, object payload);
    Task<HttpResponseMessage> DeleteAsync(string requestUri);
    Task<bool> PingApiAsync();
}