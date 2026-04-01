namespace Pudra.Connect.App.Services.Interfaces;

public interface IConnectivityService
{
    Task<bool> IsApiReachableAsync();
}