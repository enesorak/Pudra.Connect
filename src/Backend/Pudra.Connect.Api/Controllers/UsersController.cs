using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pudra.Connect.Application.Dtos.Users;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Bu Controller'ın tüm metotları sadece Admin yetkisi gerektirir.
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        // Token'dan istek yapan kullanıcının ID'sini alıyoruz.
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        // Servise bu ID'yi gönderiyoruz.
        var users = await _userService.GetAllUsersAsync(currentUserId);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto userDto)
    {
        var newUser = await _userService.CreateUserAsync(userDto);
        if (newUser == null)
        {
            return BadRequest(new { message = "Username already exists or role is invalid." });
        }
        return CreatedAtAction(nameof(GetAllUsers), new { id = newUser.Id }, newUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success)
        {
            return NotFound(new { message = "User not found." });
        }
        return NoContent(); // Başarılı silme işleminde (204 No Content) döneriz.
    }
    
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequestDto userDto)
    {
        var success = await _userService.UpdateUserAsync(id, userDto);
        if (!success)
        {
            return NotFound(new { message = "User not found or role is invalid." });
        }
        return NoContent(); // Başarılı güncellemede (204 No Content) döneriz.
    }
    
    
    // Pending kullanıcıları listele
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingUsers()
    {
        var users = await _userService.GetPendingUsersAsync();
        return Ok(users);
    }

// Kullanıcıyı onayla
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveUser(string id)
    {
        var success = await _userService.ApproveUserAsync(id);
        if (!success)
            return NotFound(new { message = "User not found." });
    
        return Ok(new { message = "User approved successfully." });
    }

// Kullanıcıyı reddet
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectUser(string id)
    {
        var success = await _userService.RejectUserAsync(id);
        if (!success)
            return NotFound(new { message = "User not found." });
    
        return Ok(new { message = "User rejected." });
    }
}