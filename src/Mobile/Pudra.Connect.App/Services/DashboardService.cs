 
using Pudra.Connect.App.Models.Dashboard;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.Services;

public class DashboardService : IDashboardService
{
    private readonly IApiService _apiService;
    public DashboardService(IApiService apiService) { _apiService = apiService; }

    public Task<List<StoreKpiDto>?> GetKpisAsync() 
        => _apiService.GetAsync<List<StoreKpiDto>>("/api/dashboard/kpi");

    public Task<List<TrendPointDto>?> GetWeeklyTrendAsync() 
        => _apiService.GetAsync<List<TrendPointDto>>("/api/dashboard/weekly-trend");

    public Task<DetailedReportDto?> GetDetailedReportAsync(DateTime startDate, DateTime endDate)
    {
        var formattedStartDate = startDate.ToString("yyyy-MM-dd");
        var formattedEndDate = endDate.ToString("yyyy-MM-dd");
        string requestUri = $"/api/dashboard/detailed-report?startDate={formattedStartDate}&endDate={formattedEndDate}";
        return _apiService.GetAsync<DetailedReportDto>(requestUri);
    }
}