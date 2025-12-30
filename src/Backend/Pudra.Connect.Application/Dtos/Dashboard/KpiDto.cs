namespace Pudra.Connect.Application.Dtos.Dashboard;

public class KpiDto
{
    
    public decimal TodaysTurnover { get; set; }
    public int TodaysSaleCount { get; set; }
    public decimal TodaysReturnAmount { get; set; }
    public int TodaysReturnCount { get; set; }
    public decimal TodaysDiscount { get; set; }

    // YENİ EKLENEN HESAPLANMIŞ ÖZELLİK
    // (TodaysReturnAmount'ın backend'den negatif geldiğini varsayıyoruz)
    public decimal NetAmount => TodaysTurnover + TodaysReturnAmount;
}