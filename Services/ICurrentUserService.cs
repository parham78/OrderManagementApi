public interface ICurrentUserService
{
    string? GetUserId();
    Task<int?> GetCustomerId();
}