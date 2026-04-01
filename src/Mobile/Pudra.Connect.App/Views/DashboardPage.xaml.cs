using System.ComponentModel;
using Pudra.Connect.App.ViewModels;

namespace Pudra.Connect.App.Views;
public partial class DashboardPage : ContentPage
{
    // Animasyonla ilgili tüm alanları ve metotları kaldırdık.
    // Sadece ViewModel'i tutuyoruz.
    private readonly DashboardViewModel _viewModel;
 

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    /// <summary>
    /// Bu metot, sayfa her göründüğünde verileri otomatik olarak yüklemek için kullanılır.
    /// Bu, doğru ve gerekli olan tek mantıktır.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
         
        // Eğer ViewModel'in komutu çalıştırılabiliyorsa, çalıştır.
        if (_viewModel.LoadDataCommand.CanExecute(null))
        {
            _viewModel.LoadDataCommand.Execute(null);
        }
    }
    
    
    
     

    

    
}