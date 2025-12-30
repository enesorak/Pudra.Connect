namespace Pudra.Connect.Application.Dtos.Dashboard;

public class StorePerformanceDto
{
    public string StoreId { get; set; }
    public decimal TotalSales { get; set; }
    public int SaleCount { get; set; }
    public decimal TotalReturns { get; set; }
    public int ReturnCount { get; set; }
    
    public decimal TotalDiscount { get; set; }
}