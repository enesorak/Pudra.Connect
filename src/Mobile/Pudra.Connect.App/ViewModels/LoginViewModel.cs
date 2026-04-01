using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _service;

    
    [ObservableProperty]
    private bool rememberMe;
    
    [ObservableProperty]
    private string username;

    [ObservableProperty]
    private string password;

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
        
                // YENİ EKLENDİ: Rolü de kaydet
                var userRole = JwtParser.GetRoleFromToken(loginResponse.Token);
                if (userRole != null)
                {
                    await SecureStorage.Default.SetAsync("user_role", userRole);
                }
                await SecureStorage.Default.SetAsync("user_fullname", loginResponse.FullName);
                
                if (RememberMe)
                {
                    await SecureStorage.SetAsync("remembered_username", Username);
                    await SecureStorage.SetAsync("remembered_password", Password);
                }
                else
                {
                    SecureStorage.Remove("remembered_username");
                    SecureStorage.Remove("remembered_password");
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
            // Timeout hatası
            await Shell.Current.DisplayAlert("Hata", "Sunucudan cevap alınamadı. Lütfen internet bağlantınızı veya sunucu durumunu kontrol edin.", "Tamam");
        }
        catch (HttpRequestException)
        {
            // Genel ağ veya bağlantı hatası
            await Shell.Current.DisplayAlert("Hata", "Sunucuya ulaşılamıyor. Lütfen internet bağlantınızı kontrol edin.", "Tamam");
        }
        catch (System.Exception ex)
        {
            // Diğer tüm beklenmedik hatalar
            await Shell.Current.DisplayAlert("Hata", $"Beklenmedik bir hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private async Task LoadRememberedCredentials()
    {
        Username = await SecureStorage.GetAsync("remembered_username") ?? string.Empty;
        Password = await SecureStorage.GetAsync("remembered_password") ?? string.Empty;
        RememberMe = !string.IsNullOrEmpty(Username);
    }
    
    [RelayCommand]
    private void Unfocus()
    {
        // Bu komut, odaklanmış olan herhangi bir kontrolün odağını kaldırarak klavyeyi kapatır.
        // Bu komutu tetiklemek için bir UI elemanına ihtiyacımız var,
        // ancak GestureRecognizer ile doğrudan bir kontrolü hedefleyemeyiz.
        // En temiz yöntem, bu işlevi Page'in kendisinde yönetmektir.
        // Bu nedenle, bu komut yerine GestureRecognizer'ı doğrudan Page'de yöneteceğiz.
        // Ancak, MVVM saflığı için burada bırakıyorum.
        // Düzeltme: En temiz yöntem, page'in code-behind'ında yönetmektir.
        // `UnfocusCommand`'ı siliyorum ve bu mantığı doğrudan jest tanıyıcıda ele alıyorum.
    }
}