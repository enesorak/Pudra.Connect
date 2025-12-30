using Microsoft.AspNetCore.Mvc;

namespace Pudra.Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        // Bu endpoint'e ulaşılabiliyorsa, sunucu ayaktadır.
        return Ok("Pong");
    }
}