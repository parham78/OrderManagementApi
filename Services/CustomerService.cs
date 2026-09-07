using Microsoft.EntityFrameworkCore;

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

        await _context.SaveChangesAsync();

        return customer;
    }
}