namespace Pudra.Connect.Application.Dtos.Analytics;

public class UserScanStatsDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalScans { get; set; }
    public int FoundScans { get; set; }
    public int NotFoundScans { get; set; }
    public DateTime? LastScanAt { get; set; }
}