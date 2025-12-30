namespace Pudra.Connect.Application.Dtos.Dashboard;

public class StoreKpiDto
{
    public string StoreId { get; set; } // YENİ EKLENDİ
    public decimal TodaysTurnover { get; set; }
    public int TodaysSaleCount { get; set; }
    public decimal TodaysReturnAmount { get; set; }
    public int TodaysReturnCount { get; set; }
    public decimal TodaysDiscount { get; set; }
}