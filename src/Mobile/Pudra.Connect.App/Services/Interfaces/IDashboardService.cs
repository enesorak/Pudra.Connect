using Pudra.Connect.App.Models.Dashboard;

namespace Pudra.Connect.App.Services.Interfaces;

public interface IDashboardService
{
    Task<List<StoreKpiDto>?> GetKpisAsync();
    Task<List<TrendPointDto>?> GetWeeklyTrendAsync();
    Task<DetailedReportDto?> GetDetailedReportAsync(DateTime startDate, DateTime endDate);
}