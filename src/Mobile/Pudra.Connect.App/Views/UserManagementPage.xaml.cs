using Pudra.Connect.App.ViewModels;

namespace Pudra.Connect.App.Views;

public partial class UserManagementPage : ContentPage
{
    private readonly UserManagementViewModel _viewModel;
  

    public UserManagementPage(UserManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        
       

    }

    // Sayfa her göründüğünde kullanıcı listesini otomatik olarak yükle
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.LoadUsersCommand.CanExecute(null))
        {
            _viewModel.LoadUsersCommand.Execute(null);
        }
    }
    
     

     

    
}