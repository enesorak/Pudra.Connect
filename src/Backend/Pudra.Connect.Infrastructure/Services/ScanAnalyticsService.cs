using Microsoft.EntityFrameworkCore;
using Pudra.Connect.Application.Dtos.Analytics;
using Pudra.Connect.Application.Interfaces;
using Pudra.Connect.Domain.Entities;
using Pudra.Connect.Domain.Enums;
using Pudra.Connect.Infrastructure.Persistence;

namespace Pudra.Connect.Infrastructure.Services;

public class ScanAnalyticsService : IScanAnalyticsService
{
    private readonly AppDbContext _context;

    public ScanAnalyticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogScanAsync(string userId, string barcode, string? productCode, string? productName, bool found)
    {
        var scanLog = new ScanLog
        {
            UserId = userId,
            Barcode = barcode,
            ProductCode = productCode,
            ProductName = productName,
            Found = found,
            ScannedAt = DateTime.UtcNow
        };

        await _context.ScanLogs.AddAsync(scanLog);
        await _context.SaveChangesAsync();
    }

    public async Task<AnalyticsSummaryDto> GetSummaryAsync()
    {
        var today = DateTime.UtcNow.Date;
        var weekAgo = today.AddDays(-7);

        var totalScansToday = await _context.ScanLogs
            .CountAsync(s => s.ScannedAt.Date == today);

        var totalScansThisWeek = await _context.ScanLogs
            .CountAsync(s => s.ScannedAt.Date >= weekAgo);

        var totalScansAllTime = await _context.ScanLogs.CountAsync();

        var activeUsersToday = await _context.ScanLogs
            .Where(s => s.ScannedAt.Date == today)
            .Select(s => s.UserId)
            .Distinct()
            .CountAsync();

        var pendingUsersCount = await _context.Users
            .CountAsync(u => u.Status == UserStatus.Pending);

        var approvedUsersCount = await _context.Users
            .CountAsync(u => u.Status == UserStatus.Approved);

        var foundScans = await _context.ScanLogs.CountAsync(s => s.Found);
        var foundRate = totalScansAllTime > 0 ? (double)foundScans / totalScansAllTime * 100 : 0;

        return new AnalyticsSummaryDto
        {
            TotalScansToday = totalScansToday,
            TotalScansThisWeek = totalScansThisWeek,
            TotalScansAllTime = totalScansAllTime,
            ActiveUsersToday = activeUsersToday,
            PendingUsersCount = pendingUsersCount,
            ApprovedUsersCount = approvedUsersCount,
            FoundRate = Math.Round(foundRate, 1)
        };
    }

    public async Task<IEnumerable<UserScanStatsDto>> GetUserStatsAsync()
    {
        var userStats = await _context.Users
            .Select(u => new UserScanStatsDto
            {
                UserId = u.Id,
                FullName = u.FullName,
                Username = u.Username,
                Status = u.Status.ToString(),
                TotalScans = _context.ScanLogs.Count(s => s.UserId == u.Id),
                FoundScans = _context.ScanLogs.Count(s => s.UserId == u.Id && s.Found),
                NotFoundScans = _context.ScanLogs.Count(s => s.UserId == u.Id && !s.Found),
                LastScanAt = _context.ScanLogs
                    .Where(s => s.UserId == u.Id)
                    .OrderByDescending(s => s.ScannedAt)
                    .Select(s => (DateTime?)s.ScannedAt)
                    .FirstOrDefault()
            })
            .OrderByDescending(u => u.TotalScans)
            .ToListAsync();

        return userStats;
    }

    public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int count = 20)
    {
        var topProducts = await _context.ScanLogs
            .Where(s => s.Found)
            .GroupBy(s => new { s.Barcode, s.ProductCode, s.ProductName })
            .Select(g => new TopProductDto
            {
                Barcode = g.Key.Barcode,
                ProductCode = g.Key.ProductCode,
                ProductName = g.Key.ProductName,
                ScanCount = g.Count(),
                UniqueUsers = g.Select(s => s.UserId).Distinct().Count()
            })
            .OrderByDescending(p => p.ScanCount)
            .Take(count)
            .ToListAsync();

        return topProducts;
    }

    public async Task<IEnumerable<ScanTrendDto>> GetDailyTrendAsync(int days = 30)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days + 1);

        var trend = await _context.ScanLogs
            .Where(s => s.ScannedAt.Date >= startDate)
            .GroupBy(s => s.ScannedAt.Date)
            .Select(g => new ScanTrendDto
            {
                Date = g.Key,
                TotalScans = g.Count(),
                UniqueUsers = g.Select(s => s.UserId).Distinct().Count()
            })
            .OrderBy(t => t.Date)
            .ToListAsync();

        return trend;
    }

    public async Task<IEnumerable<RecentScanDto>> GetRecentScansAsync(int count = 50)
    {
        var recentScans = await _context.ScanLogs
            .Include(s => s.User)
            .OrderByDescending(s => s.ScannedAt)
            .Take(count)
            .Select(s => new RecentScanDto
            {
                UserFullName = s.User != null ? s.User.FullName : "Unknown",
                Barcode = s.Barcode,
                ProductName = s.ProductName,
                Found = s.Found,
                ScannedAt = s.ScannedAt
            })
            .ToListAsync();

        return recentScans;
    }
}