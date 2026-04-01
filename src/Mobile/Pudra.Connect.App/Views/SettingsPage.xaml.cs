using Pudra.Connect.App.ViewModels;

namespace Pudra.Connect.App.Views;
public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // ViewModel'deki veri yükleme metodunu çağırıyoruz.
        await _viewModel.LoadUserDataAsync();
    }
    
    
     
}