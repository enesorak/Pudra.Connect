namespace Pudra.Connect.Application.Dtos.Analytics;

public class AnalyticsSummaryDto
{
    public int TotalScansToday { get; set; }
    public int TotalScansThisWeek { get; set; }
    public int TotalScansAllTime { get; set; }
    public int ActiveUsersToday { get; set; }
    public int PendingUsersCount { get; set; }
    public int ApprovedUsersCount { get; set; }
    public double FoundRate { get; set; } // Yüzde olarak
}