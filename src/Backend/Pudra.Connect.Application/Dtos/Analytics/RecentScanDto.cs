namespace Pudra.Connect.Application.Dtos.Analytics;

public class RecentScanDto
{
    public string UserFullName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public bool Found { get; set; }
    public DateTime ScannedAt { get; set; }
}