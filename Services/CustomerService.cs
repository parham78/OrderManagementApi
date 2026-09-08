using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

public class CustomerService : ICustomerService
{
    private readonly OrderManagementDbContext _context;

    public CustomerService(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAll()
    {
        return await _context.Customers
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Customer> GetById(int id)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null)
        {
            throw new CustomerNotFoundException(
                $"Customer {id} was not found.");
        }

        return customer;
    }

    public async Task<Customer> Create(
    CreateCustomerRequestDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            IsActive = dto.IsActive
        };

        _context.Customers.Add(customer);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException sqlException
            && (sqlException.Number == 2601 || sqlException.Number == 2627)
            && sqlException.Message.Contains(
                "IX_Customers_Email",
                StringComparison.Ordinal))
        {
            throw new ConflictException(
                "A customer with this email already exists.",
                ex);
        }

        return customer;
    }
}