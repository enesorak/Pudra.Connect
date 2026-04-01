using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Models.Users;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.ViewModels;

[QueryProperty(nameof(UserToEdit), "UserToEdit")]
public partial class UserDetailViewModel : ObservableObject
{
    private readonly IUserService _service;

    // Form alanları
    [ObservableProperty] private string username = "";
    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string selectedRole = "Seller";
    public List<string> Roles { get; } = new List<string> { "Seller", "Admin" };

    // Sayfa durumu
    [ObservableProperty] private string pageTitle = "Yeni Kullanıcı Ekle";
    [ObservableProperty] private bool isEditMode = false;
    private string? userIdToUpdate;
    
    // Düzenlenecek kullanıcı bu özelliğe atanacak.
    [ObservableProperty]
    UserDto? userToEdit;

    [ObservableProperty] private bool isBusy;

    public UserDetailViewModel(IUserService service)
    {
        _service = service;
    }

    // Bu metot, UserToEdit özelliği her değiştiğinde (yani düzenleme için veri geldiğinde) çalışır.
    partial void OnUserToEditChanged(UserDto? value)
    {
        if (value != null)
        {
            IsEditMode = true;
            PageTitle = "Kullanıcıyı Düzenle";
            userIdToUpdate = value.Id;
            Username = value.Username;
            FullName = value.FullName;
            SelectedRole = value.Role;
            // Şifre alanı güvenlik için boş bırakılır. Değiştirilmek istenirse doldurulur.
            Password = "";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;
        // ... (form doğrulama kontrolleri aynı)

        IsBusy = true;
        try
        {
            if (IsEditMode) // EĞER DÜZENLEME MODUNDAYSAK
            {
                var request = new UpdateUserRequestDto
                {
                    FullName = FullName,
                    Role = SelectedRole,
                    Password = string.IsNullOrWhiteSpace(Password) ? null : Password // Şifre boşsa null gönder
                };
                bool success = await _service.UpdateUserAsync(userIdToUpdate!, request);
                await HandleApiResponse(success, "Kullanıcı güncellendi.", "Kullanıcı güncellenemedi.");
            }
            else // EĞER YENİ KULLANICI MODUNDAYSAK
            {
                var request = new CreateUserRequestDto
                {
                    Username = Username,
                    FullName = FullName,
                    Password = Password,
                    Role = SelectedRole
                };
                var createdUser = await _service.CreateUserAsync(request);
                await HandleApiResponse(createdUser != null, "Kullanıcı oluşturuldu.", "Kullanıcı adı zaten mevcut.");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", $"Beklenmedik bir hata oluştu: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    // Kod tekrarını önlemek için yardımcı metot
    private async Task HandleApiResponse(bool success, string successMessage, string errorMessage)
    {
        if (success)
        {
            await Shell.Current.DisplayAlert("Başarılı", successMessage, "Tamam");
            await Shell.Current.GoToAsync(".."); // Bir önceki sayfaya dön
        }
        else
        {
            await Shell.Current.DisplayAlert("Hata", errorMessage, "Tamam");
        }
    } }