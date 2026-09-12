using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(
    IAuthService authService,
    ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        var result = await _authService.Register(dto);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new
        {
            message = "User registered successfully."
        });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _authService.Login(dto);

        if (result == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok(new
        {
            message = "You are authenticated."
        });
    }
    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("admin-only")]
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            message = "You are an admin."
        });
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = _currentUserService.GetUserId();
        var customerId = await _currentUserService.GetCustomerId();

        return Ok(new
        {
            userId,
            customerId
        });
    }
}