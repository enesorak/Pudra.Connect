using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Models.Profile;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.ViewModels;

public partial class MyProfileViewModel : ObservableObject
    {
        private readonly IProfileService _service;

        [ObservableProperty] private string fullName;
        [ObservableProperty] private string oldPassword;
        [ObservableProperty] private string newPassword;
        [ObservableProperty] private bool isBusy;

        public MyProfileViewModel(IProfileService service)
        {
            _service = service;
            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            FullName = await SecureStorage.Default.GetAsync("user_fullname") ?? "";
        }

        [RelayCommand]
        private async Task UpdateProfileAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(FullName)) return;
            IsBusy = true;
            var success = await _service.UpdateMyProfileAsync(new UpdateProfileRequestDto { FullName = FullName });
            if (success)
            {
                await SecureStorage.Default.SetAsync("user_fullname", FullName);
                await Shell.Current.DisplayAlert("Başarılı", "Profil bilgileri güncellendi.", "Tamam");
            }
            else
            {
                await Shell.Current.DisplayAlert("Hata", "Profil güncellenemedi.", "Tamam");
            }
            IsBusy = false;
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword)) return;
            IsBusy = true;
            var success = await _service.ChangeMyPasswordAsync(new ChangePasswordRequestDto { OldPassword = OldPassword, NewPassword = NewPassword });
            if (success)
            {
                await Shell.Current.DisplayAlert("Başarılı", "Şifreniz başarıyla değiştirildi.", "Tamam");
                OldPassword = "";
                NewPassword = "";
            }
            else
            {
                await Shell.Current.DisplayAlert("Hata", "Eski şifre yanlış veya bir hata oluştu.", "Tamam");
            }
            IsBusy = false;
        }
    }