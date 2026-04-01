namespace Pudra.Connect.Application.Dtos.Analytics;

public class TopProductDto
{
    public string Barcode { get; set; } = string.Empty;
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public int ScanCount { get; set; }
    public int UniqueUsers { get; set; }
}