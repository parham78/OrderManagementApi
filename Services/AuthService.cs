using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<IdentityResult> Register(RegisterRequestDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(
            user,
            dto.Password);

        return result;
    }
}