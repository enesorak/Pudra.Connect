using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Bu endpoint'lere sadece Admin erişebilir.
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("kpi")]
    public async Task<IActionResult> GetKpis()
    {
        var kpis = await _dashboardService.GetKpisAsync();
        return Ok(kpis);
    }

    [HttpGet("weekly-trend")]
    public async Task<IActionResult> GetWeeklyTrend()
    {
        var trendData = await _dashboardService.GetWeeklyTrendAsync();
        return Ok(trendData);
    }

    [HttpGet("detailed-report")]
    public async Task<IActionResult> GetDetailedReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest("Start date cannot be after end date.");
        
        var report = await _dashboardService.GetDetailedReportAsync(startDate, endDate);
        return Ok(report);
    }
    
    [HttpGet("category-performance")]
    public async Task<IActionResult> GetCategoryPerformance([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest("Start date cannot be after end date.");
    
        var report = await _dashboardService.GetCategoryPerformanceAsync(startDate, endDate);
        return Ok(report);
    }
}