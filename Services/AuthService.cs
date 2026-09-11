using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly OrderManagementDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(
    UserManager<ApplicationUser> userManager,
    OrderManagementDbContext context,
    ITokenService tokenService)
    {
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
    }
    public async Task<IdentityResult> Register(RegisterRequestDto dto)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var createUserResult =
            await _userManager.CreateAsync(user, dto.Password);

        if (!createUserResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return createUserResult;
        }

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                AppRoles.Customer);

        if (!roleResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return roleResult;
        }

        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            IsActive = true,
            UserId = user.Id
        };

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return IdentityResult.Success;
    }
    public async Task<LoginResponseDto?> Login(LoginRequestDto dto)
    {
        var user =
            await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            return null;
        }

        var passwordValid =
            await _userManager.CheckPasswordAsync(
                user,
                dto.Password);

        if (!passwordValid)
        {
            return null;
        }

        var token =
            await _tokenService.CreateToken(user);

        return new LoginResponseDto
        {
            Token = token
        };
    }
}