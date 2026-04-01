using Pudra.Connect.App.ViewModels;

namespace Pudra.Connect.App.Views;

public partial class DetailedReportPage : ContentPage
{
    public DetailedReportPage(DetailedReportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}