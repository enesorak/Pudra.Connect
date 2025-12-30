using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pudra.Connect.Application.Dtos.Profile;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize] // Bu endpoint'lere SADECE giriş yapmış bir kullanıcı erişebilir.
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;

    public ProfileController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var success = await _userService.UpdateMyProfileAsync(userId, request.FullName);
        return success ? NoContent() : NotFound();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangeMyPassword([FromBody] ChangePasswordRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var success = await _userService.ChangeMyPasswordAsync(userId, request.OldPassword, request.NewPassword);
        return success ? Ok(new { message = "Password updated successfully." }) : BadRequest(new { message = "Incorrect old password." });
    }
}