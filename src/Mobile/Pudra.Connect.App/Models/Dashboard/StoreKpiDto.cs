namespace Pudra.Connect.App.Models.Dashboard;

public class StoreKpiDto
{
    public string? StoreId { get; set; }
    public decimal TodaysTurnover { get; set; }
    public int TodaysSaleCount { get; set; }
    public decimal TodaysReturnAmount { get; set; }
    public int TodaysReturnCount { get; set; }
    public decimal TodaysDiscount { get; set; }

    /// <summary>
    /// YENİ EKLENEN HESAPLANMIŞ ÖZELLİK
    /// Net ciroyu hesaplar (İadelerin negatif geldiği varsayımıyla).
    /// </summary>
    public decimal NetAmount => TodaysTurnover + TodaysReturnAmount;
}