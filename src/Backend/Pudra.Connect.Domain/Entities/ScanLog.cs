namespace Pudra.Connect.Domain.Entities;

public class ScanLog : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public bool Found { get; set; } // Ürün bulundu mu?
    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User? User { get; set; }
}