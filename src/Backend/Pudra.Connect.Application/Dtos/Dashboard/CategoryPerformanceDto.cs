namespace Pudra.Connect.Application.Dtos.Dashboard;

public class CategoryPerformanceDto
{
    public string CategoryName { get; set; }
    public decimal TotalSales { get; set; }
    public int SaleCount { get; set; }
    public decimal TotalReturns { get; set; }
    public int ReturnCount { get; set; }
}