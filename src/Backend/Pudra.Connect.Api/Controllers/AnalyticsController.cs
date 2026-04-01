using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : ControllerBase
{
    private readonly IScanAnalyticsService _analyticsService;

    public AnalyticsController(IScanAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _analyticsService.GetSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUserStats()
    {
        var stats = await _analyticsService.GetUserStatsAsync();
        return Ok(stats);
    }

    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopProducts([FromQuery] int count = 20)
    {
        var products = await _analyticsService.GetTopProductsAsync(count);
        return Ok(products);
    }

    [HttpGet("trend")]
    public async Task<IActionResult> GetDailyTrend([FromQuery] int days = 30)
    {
        var trend = await _analyticsService.GetDailyTrendAsync(days);
        return Ok(trend);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentScans([FromQuery] int count = 50)
    {
        var scans = await _analyticsService.GetRecentScansAsync(count);
        return Ok(scans);
    }
}