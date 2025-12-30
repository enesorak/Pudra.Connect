namespace Pudra.Connect.Application.Dtos.Dashboard;

public class DailyActivityDto
{
    public string Seller { get; set; }
    public string StoreId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public bool IsReturn { get; set; }
    public int Quantity { get; set; }
    public decimal NetAmountWithTax { get; set; }
    
    public decimal TotalDiscount { get; set; }
}