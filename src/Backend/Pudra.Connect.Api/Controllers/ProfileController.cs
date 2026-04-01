using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pudra.Connect.Application.Dtos.Profile;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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

    // YENİ: Profil resmi yükle
    [HttpPut("image")]
    public async Task<IActionResult> UploadProfileImage([FromBody] UploadProfileImageDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var success = await _userService.UpdateProfileImageAsync(userId, request.ImageBase64);
        return success ? Ok(new { message = "Profile image updated." }) : BadRequest(new { message = "Failed to update image." });
    }

    // YENİ: Kendi profilimi getir
    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var profile = await _userService.GetUserByIdAsync(userId);
        return profile != null ? Ok(profile) : NotFound();
    }
}