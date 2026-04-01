using Pudra.Connect.App.ViewModels;

namespace Pudra.Connect.App.Views;

public partial class MyProfilePage : ContentPage
{
    public MyProfilePage(MyProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}