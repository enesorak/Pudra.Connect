using Pudra.Connect.Application.Dtos.Dashboard;

namespace Pudra.Connect.Application.Interfaces;

public interface IDashboardRepository
{
    Task<IEnumerable<DailyActivityDto>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    Task<KpiDto> GetTodaysKpisAsync(); // Yeni
    Task<IEnumerable<TrendPointDto>> GetWeeklyTrendAsync(); // Yeni


    Task<IEnumerable<StoreKpiDto>> GetTodaysKpisByStoreAsync();
    Task<IEnumerable<CategoryPerformanceDto>> GetCategoryPerformanceAsync(DateTime startDate, DateTime endDate);


}