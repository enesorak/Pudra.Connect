using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Services.Interfaces;
using Pudra.Connect.App.Views;

namespace Pudra.Connect.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IProfileService _profileService;
    
    [ObservableProperty]
    private string userName = "Yükleniyor...";

    [ObservableProperty]
    private string userEmail = "";

    [ObservableProperty]
    private string userRole = "";
    
    [ObservableProperty]
    private bool isAdmin;
    
    [ObservableProperty]
    private string profileImageUrl = "default_avatar.png";
    
    [ObservableProperty]
    private string statusText = "";
    
    [ObservableProperty]
    private Color statusColor = Colors.Gray;
    
    [ObservableProperty]
    private int remainingScans = -1;
    
    [ObservableProperty]
    private string remainingScansText = "";
    
    [ObservableProperty]
    private string scanLimitDescription = "";
    
    [ObservableProperty]
    private int pendingUsersCount = 0;
    
    [ObservableProperty]
    private bool hasPendingUsers = false;

    public SettingsViewModel(IProfileService profileService)
    {
        _profileService = profileService;
    }

    public async Task LoadUserDataAsync()
    {
        try
        {
            // Token'dan bilgileri al
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return;

            UserName = await SecureStorage.Default.GetAsync("user_fullname") ?? "Kullanıcı";
            UserRole = await SecureStorage.Default.GetAsync("user_role") ?? "";
            UserEmail = await SecureStorage.Default.GetAsync("user_email") ?? "";
            
            var status = await SecureStorage.Default.GetAsync("user_status") ?? "Approved";
            var scansStr = await SecureStorage.Default.GetAsync("remaining_scans");
            
            IsAdmin = UserRole == "Admin";
            
            // Durum ayarla
            if (status == "Approved")
            {
                StatusText = "Onaylı Hesap";
                StatusColor = Colors.Green;
                RemainingScansText = "∞";
                ScanLimitDescription = "Sınırsız tarama hakkınız var";
            }
            else if (status == "Pending")
            {
                StatusText = "Onay Bekliyor";
                StatusColor = Colors.Orange;
                
                if (int.TryParse(scansStr, out var scans))
                {
                    RemainingScans = scans;
                    RemainingScansText = scans.ToString();
                    ScanLimitDescription = $"50 ücretsiz taramadan {50 - scans} kullanıldı";
                }
                else
                {
                    RemainingScansText = "50";
                    ScanLimitDescription = "50 ücretsiz tarama hakkınız var";
                }
            }
            
            // Profil resmi
            var imageUrl = await SecureStorage.Default.GetAsync("profile_image_url");
            if (!string.IsNullOrEmpty(imageUrl))
            {
                ProfileImageUrl = imageUrl;
            }
            
            // Admin ise pending users sayısını al
            if (IsAdmin)
            {
                // TODO: API'den pending users count al
                // PendingUsersCount = await _userService.GetPendingUsersCountAsync();
                // HasPendingUsers = PendingUsersCount > 0;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading user data: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ChangeProfileImageAsync()
    {
        try
        {
            var action = await Shell.Current.DisplayActionSheet(
                "Profil Fotoğrafı", 
                "İptal", 
                null, 
                "Galeriden Seç", 
                "Fotoğraf Çek");

            if (action == "Galeriden Seç")
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Profil fotoğrafı seçin"
                });

                if (result != null)
                {
                    await UploadProfileImageAsync(result);
                }
            }
            else if (action == "Fotoğraf Çek")
            {
                var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Profil fotoğrafı çekin"
                });

                if (result != null)
                {
                    await UploadProfileImageAsync(result);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", $"Fotoğraf seçilemedi: {ex.Message}", "Tamam");
        }
    }

    private async Task UploadProfileImageAsync(FileResult photo)
    {
        try
        {
            using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            var base64 = $"data:image/{photo.ContentType.Split('/').Last()};base64,{Convert.ToBase64String(bytes)}";

            var success = await _profileService.UpdateProfileImageAsync(base64);
            if (success)
            {
                ProfileImageUrl = base64;
                await SecureStorage.Default.SetAsync("profile_image_url", base64);
                await Shell.Current.DisplayAlert("Başarılı", "Profil fotoğrafı güncellendi.", "Tamam");
            }
            else
            {
                await Shell.Current.DisplayAlert("Hata", "Fotoğraf yüklenemedi.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", $"Fotoğraf yüklenirken hata: {ex.Message}", "Tamam");
        }
    }

    [RelayCommand]
    private async Task GoToEditProfileAsync()
    {
        await Shell.Current.GoToAsync(nameof(MyProfilePage));
    }

    [RelayCommand]
    private async Task GoToChangePasswordAsync()
    {
        await Shell.Current.GoToAsync(nameof(MyProfilePage));
    }

    [RelayCommand]
    private async Task GoToUserManagementAsync()
    {
        await Shell.Current.GoToAsync(nameof(UserManagementPage));
    }

    [RelayCommand]
    private async Task GoToPendingUsersAsync()
    {
        // TODO: PendingUsersPage oluştur
        await Shell.Current.GoToAsync(nameof(UserManagementPage));
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        SecureStorage.Default.RemoveAll();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}