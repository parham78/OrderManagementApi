using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


public class ProductService : IProductService
{
    private readonly OrderManagementDbContext _context;

    public ProductService(OrderManagementDbContext context)
    {
        _context = context;
    }
    public async Task<List<Product>> GetAll()
    {
        return await _context.Products
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Product?> GetById(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return product;
    }
    public async Task<Product?> GetByName(string name)
    {
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Name == name);
        return product;
    }
    public async Task<List<Product>> GetExpensiveProducts(decimal minimumPrice)
    {
        var products = await _context.Products
       .AsNoTracking()
       .Where(p => p.Price > minimumPrice)
       .OrderBy(p => p.Price)
    .ToListAsync();

        return products;
    }
    public async Task<Product> Create(CreateProductRequestDto dto)
    {
        var product = new Product
        {

            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();


        // return it
        return product;

    }
    public async Task<Product?> UpdateStock(int id, int newStock)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return null;
        }
        // Console.WriteLine(_context.Entry(product).State);
        product.Stock = newStock;
        // Console.WriteLine(_context.Entry(product).State);
        await _context.SaveChangesAsync();
        return product;

        // we'll continue here
    }
    public async Task<bool> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return false;
        }

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return true;
    }

}