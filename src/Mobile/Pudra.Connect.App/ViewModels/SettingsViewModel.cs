using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Views;

namespace Pudra.Connect.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string userName = "Yükleniyor...";

    [ObservableProperty]
    private string userRole = "Yükleniyor...";
    
    [ObservableProperty]
    private bool isAdmin; // Admin yetkisini tutan özellik

     
    
      public async Task LoadUserDataAsync()
            {
                var token = await SecureStorage.Default.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                { 
                    UserRole = JwtParser.GetRoleFromToken(token) ?? "Bilinmiyor"; 
                    UserName = await SecureStorage.Default.GetAsync("user_fullname");
                    
                    var userRole = await SecureStorage.GetAsync("user_role");
                    IsAdmin = userRole == "Admin";
                }
            }

    public async Task InitializeAsync()
    {
        // Token'ı çözerek bilgileri alalım.
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            // JWT Parser'dan kullanıcı adını ve rolünü al
            // Not: JWT Parser'a FullName için de bir metot eklememiz gerekebilir.
            // Şimdilik rolü gösterelim.
            UserRole = JwtParser.GetRoleFromToken(token) ?? "Bilinmiyor";
            UserName = "Kullanıcı Adı (JWT'den)"; // Bu kısmı detaylandıracağız.
        }
    }
        
    [RelayCommand]
    private async Task GoToUserManagementAsync()
    {
        Debug.WriteLine("User Management command executed!"); // Test logu
        await Shell.Current.GoToAsync(nameof(UserManagementPage));
    }
        
    [RelayCommand]
    private async Task LogoutAsync()
    {
        Debug.WriteLine("Logout command executed!"); // Test logu
        SecureStorage.Default.Remove("auth_token");
        SecureStorage.Default.Remove("user_role");
        SecureStorage.Default.Remove("user_fullname");
        await Shell.Current.GoToAsync("//LoginPage");
    }
    
    [RelayCommand]
    private async Task GoToMyProfileAsync()
    {
        await Shell.Current.GoToAsync(nameof(MyProfilePage));
    }
}