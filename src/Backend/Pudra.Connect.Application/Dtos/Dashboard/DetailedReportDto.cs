namespace Pudra.Connect.Application.Dtos.Dashboard;

public class DetailedReportDto
{
    public List<StorePerformanceDto> StoreReport { get; set; } = new();
    public List<SellerPerformanceDto> SellerReport { get; set; } = new();
}