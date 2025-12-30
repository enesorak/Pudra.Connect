namespace Pudra.Connect.Application.Dtos.Dashboard;

public class SellerPerformanceDto
{
    public string Seller { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int SaleCount { get; set; }
}