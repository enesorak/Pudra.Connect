using Microsoft.AspNetCore.Mvc;

namespace Pudra.Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // Bu yolun "api/test" olmasını sağlar
public class TestController : ControllerBase
{
    // Bu endpoint'e tarayıcıdan GET isteği ile ulaşılabilir.
    [HttpGet("ping")] // Bu yolun "api/test/ping" olmasını sağlar
    public IActionResult Ping()
    {
        // Eğer bu endpoint'e ulaşabilirsek, geriye bu metni dönecek.
        return Ok("Pong! Sunucu çalışıyor ve bu endpoint'e ulaşılabiliyor.");
    }
}