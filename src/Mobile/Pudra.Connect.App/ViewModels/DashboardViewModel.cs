using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using SkiaSharp;
using Pudra.Connect.App.Models.Dashboard;
using Pudra.Connect.App.Services.Interfaces;
using Pudra.Connect.App.Views;

namespace Pudra.Connect.App.ViewModels;

 
  public partial class DashboardViewModel : ObservableObject
    {
        private readonly IDashboardService _dashboardService;
        private readonly IConnectivityService _connectivityService;

        [ObservableProperty]
        private bool isRefreshing;
        
        
        [ObservableProperty]
        private StoreKpiDto totalKpis = new() { StoreId = "TÜM MAĞAZALAR" };

        public ObservableCollection<StoreKpiDto> StoreKpis { get; } = new();

        [ObservableProperty]
        private Chart? weeklyTrendChart;
        
        [ObservableProperty]
        private bool isBusy;

        public DashboardViewModel(IDashboardService dashboardService, IConnectivityService connectivityService)
        {
            _dashboardService = dashboardService;
            _connectivityService = connectivityService;
        }
 
        [RelayCommand]
private async Task LoadDataAsync()
{
    if (IsBusy) return;

    IsBusy = true;
    IsRefreshing = true; // RefreshView spinner'ı başlasın

    try
    {
        if (!await _connectivityService.IsApiReachableAsync().ConfigureAwait(false))
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.DisplayAlert("Bağlantı Hatası", "Sunucuya ulaşılamıyor.", "Tamam");
            });
            return;
        }

        var kpiListTask = _dashboardService.GetKpisAsync();
        var trendDataTask = _dashboardService.GetWeeklyTrendAsync();

        await Task.WhenAll(kpiListTask, trendDataTask).ConfigureAwait(false);

        var kpiList = await kpiListTask;
        var trendData = await trendDataTask;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            StoreKpis.Clear();
            if (kpiList != null && kpiList.Any())
            {
                foreach (var kpi in kpiList) StoreKpis.Add(kpi);

                TotalKpis = new StoreKpiDto
                {
                    StoreId = "TÜM MAĞAZALAR",
                    TodaysTurnover = kpiList.Sum(k => k.TodaysTurnover),
                    TodaysSaleCount = kpiList.Sum(k => k.TodaysSaleCount),
                    TodaysReturnAmount = kpiList.Sum(k => k.TodaysReturnAmount),
                    TodaysReturnCount = kpiList.Sum(k => k.TodaysReturnCount),
                    TodaysDiscount = kpiList.Sum(k => k.TodaysDiscount)
                };
            }
            else
            {
                TotalKpis = new StoreKpiDto { StoreId = "TÜM MAĞAZALAR" };
            }

            if (trendData != null && trendData.Any())
            {
                var chartEntries = trendData.Select(td => new ChartEntry((float)td.Amount)
                {
                    Label = td.Date.ToString("dd MMM"),
                    ValueLabel = td.Amount.ToString("N0"),
                    Color = SKColor.Parse("#8E44AD")
                }).ToList();

                WeeklyTrendChart = new LineChart
                {
                    Entries = chartEntries,
                    LabelTextSize = 25f,
                    LineMode = LineMode.Spline,
                    PointMode = PointMode.None,
                    BackgroundColor = SKColors.Transparent,
                    IsAnimated = false
                };
            }
            else
            {
                WeeklyTrendChart = null;
            }
        });
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"[DashboardViewModel] CRITICAL ERROR: {ex}");
        // --- DEĞİŞİKLİK BURADA: Hata mesajını da ana thread'de gösteriyoruz ---
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.DisplayAlert("Hata", "Dashboard verileri yüklenirken bilinmeyen bir hata oluştu.", "Tamam");
        });    }
    finally
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            return IsBusy = false;
        });

        IsRefreshing = false; // RefreshView spinner'ı durdur
    }
}

        [RelayCommand]
        private async Task GoToDetailedReportAsync()
        {
            await Shell.Current.GoToAsync(nameof(DetailedReportPage));
        }
    }