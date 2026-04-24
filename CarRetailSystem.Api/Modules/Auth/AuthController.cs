using CarRetailSystem.Api.Modules.Auth.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRetailSystem.Api.Modules.Auth;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        if (result is null) return Unauthorized(new { message = "Invalid credentials." });
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var result = await authService.RefreshAsync(request.RefreshToken);
        if (result is null) return Unauthorized(new { message = "Invalid or expired refresh token." });
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest request)
    {
        await authService.RevokeAsync(request.RefreshToken);
        return NoContent();
    }
}
