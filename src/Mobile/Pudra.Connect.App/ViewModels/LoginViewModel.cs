using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Helpers;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _service;

    [ObservableProperty]
    private bool rememberMe;
    
    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    public LoginViewModel(IAuthService service)
    {
        _service = service;
        _ = LoadRememberedCredentials();
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Hata", "Kullanıcı adı ve şifre boş olamaz.", "Tamam");
            return;
        }

        IsBusy = true;
        try
        {
            var loginResponse = await _service.LoginAsync(Username, Password);

            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
            {
                await SecureStorage.Default.SetAsync("auth_token", loginResponse.Token);
                await SecureStorage.Default.SetAsync("user_fullname", loginResponse.FullName);
                await SecureStorage.Default.SetAsync("user_email", Username);
                await SecureStorage.Default.SetAsync("user_status", loginResponse.Status);
                await SecureStorage.Default.SetAsync("remaining_scans", loginResponse.RemainingScans.ToString());

                if (!string.IsNullOrEmpty(loginResponse.ProfileImageUrl))
                {
                    await SecureStorage.Default.SetAsync("profile_image_url", loginResponse.ProfileImageUrl);
                }

                var userRole = JwtParser.GetRoleFromToken(loginResponse.Token);
                if (userRole != null)
                {
                    await SecureStorage.Default.SetAsync("user_role", userRole);
                }
                
                if (RememberMe)
                {
                    await SecureStorage.Default.SetAsync("remembered_username", Username);
                    await SecureStorage.Default.SetAsync("remembered_password", Password);
                }
                else
                {
                    SecureStorage.Default.Remove("remembered_username");
                    SecureStorage.Default.Remove("remembered_password");
                }

                await Shell.Current.GoToAsync("//Main");
            }
            else
            {
                await Shell.Current.DisplayAlert("Giriş Başarısız", "Kullanıcı adı veya şifre hatalı.", "Tamam");
            }
        }
        catch (TaskCanceledException)
        {
            await Shell.Current.DisplayAlert("Hata", "Sunucudan cevap alınamadı. Lütfen internet bağlantınızı veya sunucu durumunu kontrol edin.", "Tamam");
        }
        catch (HttpRequestException)
        {
            await Shell.Current.DisplayAlert("Hata", "Sunucuya ulaşılamıyor. Lütfen internet bağlantınızı kontrol edin.", "Tamam");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", $"Beklenmedik bir hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private async Task LoadRememberedCredentials()
    {
        Username = await SecureStorage.Default.GetAsync("remembered_username") ?? string.Empty;
        Password = await SecureStorage.Default.GetAsync("remembered_password") ?? string.Empty;
        RememberMe = !string.IsNullOrEmpty(Username);
    }
}