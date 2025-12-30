using Pudra.Connect.Application.Dtos.Dashboard;

namespace Pudra.Connect.Application.Interfaces;

public interface IDashboardService
{


    Task<IEnumerable<StoreKpiDto>> GetKpisAsync();
    Task<IEnumerable<TrendPointDto>> GetWeeklyTrendAsync(); // Yeni
    Task<DetailedReportDto> GetDetailedReportAsync(DateTime startDate, DateTime endDate); // Değişti
    
    /// <summary>
    /// Belirtilen tarih aralığı için kategori bazlı performans raporunu alır.
    /// </summary>
    /// <param name="startDate">Raporun başlangıç tarihi.</param>
    /// <param name="endDate">Raporun bitiş tarihi.</param>
    /// <returns>Her bir kategori için satış/iade adet ve tutarlarını içeren bir liste.</returns>
    Task<IEnumerable<CategoryPerformanceDto>> GetCategoryPerformanceAsync(DateTime startDate, DateTime endDate);

}