using Pudra.Connect.Application.Dtos.Analytics;

namespace Pudra.Connect.Application.Interfaces;

public interface IScanAnalyticsService
{
    Task LogScanAsync(string userId, string barcode, string? productCode, string? productName, bool found);
    Task<AnalyticsSummaryDto> GetSummaryAsync();
    Task<IEnumerable<UserScanStatsDto>> GetUserStatsAsync();
    Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int count = 20);
    Task<IEnumerable<ScanTrendDto>> GetDailyTrendAsync(int days = 30);
    Task<IEnumerable<RecentScanDto>> GetRecentScansAsync(int count = 50);
}