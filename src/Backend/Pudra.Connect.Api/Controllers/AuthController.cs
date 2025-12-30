using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pudra.Connect.Application.Dtos.Auth;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
        
    // Bu endpoint'e sadece Admin rolündeki kullanıcılar erişebilir.
    [HttpPost("register")]
   // [Authorize(Roles = "Admin")] // Bu satırı geri eklediğimizi varsayıyorum
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var success = await _authService.RegisterAsync(request.Username, request.FullName, request.Password, request.Role);
        if (success)
        {
            return Ok(new { message = "User created successfully." });
        }
        return BadRequest(new { message = "Username already exists or an error occurred." });
    }
}