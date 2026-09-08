using Microsoft.AspNetCore.Identity;

public interface IAuthService
{
    Task<IdentityResult> Register(RegisterRequestDto dto);
}