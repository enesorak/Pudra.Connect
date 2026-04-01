using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Models.Users;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Services.Interfaces;
using Pudra.Connect.App.Views;

namespace Pudra.Connect.App.ViewModels;

public partial class UserManagementViewModel : ObservableObject
{
    private readonly IUserService _userService;

    public ObservableCollection<UserDto> Users { get; } = new();
        
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;
    public bool IsNotBusy => !IsBusy;

    public UserManagementViewModel(IUserService userService)
    {
        _userService = userService;
    }

 
    
    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        
        // IsBusy'yi UI thread'inde başlatıyoruz.
        if (!MainThread.IsMainThread)
        {
            await MainThread.InvokeOnMainThreadAsync(() => IsBusy = true);
        }
        else
        {
            IsBusy = true;
        }

        try
        {
            // DİKKAT: ConfigureAwait(false) komutu, UI thread'ini bekletme der.
            // Bu, 'deadlock'u önleyen en kritik adımdır.
            var users = await _userService.GetUsersAsync().ConfigureAwait(false);

            // UI koleksiyonunu ve özelliklerini güncellemek için
            // AÇIKÇA ve GÜVENLİ bir şekilde UI thread'ine geri dönüyoruz.
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Users.Clear();
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        Users.Add(user);
                    }
                }
                    
                // İşin bittiğini de yine UI thread'inde bildiriyoruz.
                IsBusy = false; 
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading users: {ex}");
            // Hata mesajını ve IsBusy durumunu da UI thread'inde güncelliyoruz.
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.DisplayAlert("Hata", "Kullanıcı listesi yüklenemedi.", "Tamam");
                IsBusy = false;
            });
        }
        // Artık 'finally' bloğuna ihtiyacımız yok, çünkü IsBusy'yi her iki senaryoda da
        // (try ve catch) güvenli bir şekilde UI thread'inde güncelledik.
    }
    
    
    
    [RelayCommand]
    private async Task GoToCreateUserAsync()
    {
        await Shell.Current.GoToAsync("UserDetailPage");
    }
    [RelayCommand]
    private async Task GoToEditUserAsync(UserDto user)
    {
        if (user == null) return;
    
        await Shell.Current.GoToAsync(nameof(UserDetailPage), new Dictionary<string, object>
        {
            { "UserToEdit", user }
        });
    }
    [RelayCommand]
    private async Task DeleteUserAsync(UserDto user)
    {
        if (user == null) return;
            
        bool confirmed = await Shell.Current.DisplayAlert("Kullanıcıyı Sil", $"'{user.FullName}' adlı kullanıcıyı silmek istediğinizden emin misiniz?", "Evet, Sil", "İptal");
        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var success = await _userService.DeleteUserAsync(user.Id);
            if (success)
            {
                Users.Remove(user);
            }
            else
            {
                await Shell.Current.DisplayAlert("Hata", "Kullanıcı silinemedi.", "Tamam");
            }
        }
        finally { IsBusy = false; }
    }
}