using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
}