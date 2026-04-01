namespace Pudra.Connect.Application.Dtos.Analytics;

public class ScanTrendDto
{
    public DateTime Date { get; set; }
    public int TotalScans { get; set; }
    public int UniqueUsers { get; set; }
}