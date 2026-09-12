using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly OrderManagementDbContext _context;

    public CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    OrderManagementDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public string? GetUserId()
    {
        return _httpContextAccessor
            .HttpContext?
            .User
            .FindFirst(JwtRegisteredClaimNames.Sub)?
            .Value;
    }
    public async Task<int?> GetCustomerId()
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return null;
        }

        return await _context.Customers
            .Where(c => c.UserId == userId)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
    }
}