namespace Pudra.Connect.App.Models.Dashboard;

public class DetailedReportDto
{
    public List<StorePerformanceDto> StoreReport { get; set; } = new();
    public List<SellerPerformanceDto> SellerReport { get; set; } = new();
}

public class StorePerformanceDto
{
    public string? StoreId { get; set; }
    public decimal TotalSales { get; set; }
    public int SaleCount { get; set; }
    public decimal TotalReturns { get; set; }
    public int ReturnCount { get; set; }
    public decimal TotalDiscount { get; set; } // YENİ EKLENDİ

    public decimal NetAmount => TotalSales + TotalReturns;

}
    
public class SellerPerformanceDto
{
    public string? Seller { get; set; }
    public decimal TotalSales { get; set; }
    public int SaleCount { get; set; }
    public decimal TotalDiscount { get; set; } // YENİ EKLENDİ

    public decimal NetAmount => TotalSales; // Bu, backend güncellenince değişecek.

}