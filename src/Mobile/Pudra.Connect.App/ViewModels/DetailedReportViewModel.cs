using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Models.Dashboard;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.ViewModels;

public partial class DetailedReportViewModel : ObservableObject
{
    private readonly IDashboardService _service;

    [ObservableProperty]
    private DateTime startDate;

    [ObservableProperty]
    private DateTime endDate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    public ObservableCollection<StorePerformanceDto> StoreReport { get; } = new();
    public ObservableCollection<SellerPerformanceDto> SellerReport { get; } = new();

    public DetailedReportViewModel(IDashboardService service)
    {
        _service = service;
        EndDate = DateTime.Now;
        StartDate = EndDate.AddDays(-6);
    }

    [RelayCommand]
    private async Task LoadReportAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StoreReport.Clear();
        SellerReport.Clear();

        try
        {
            var reportData = await _service.GetDetailedReportAsync(StartDate, EndDate);
            if (reportData != null)
            {
                foreach (var store in reportData.StoreReport)
                {
                    StoreReport.Add(store);
                }

                foreach (var seller in reportData.SellerReport)
                {
                    SellerReport.Add(seller);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Report could not be loaded: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}