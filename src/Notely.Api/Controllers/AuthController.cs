using Microsoft.AspNetCore.Mvc;
using Notely.Api.DTOs.Auth;
using Notely.Api.Services;

namespace Notely.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var result = await authService.RegisterAsync(req);
        if (result is null)
            return Conflict(new { message = "Email already in use." });

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var result = await authService.LoginAsync(req);
        if (result is null)
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(result);
    }
}
