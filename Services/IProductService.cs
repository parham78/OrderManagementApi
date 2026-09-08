public interface IProductService
{
    Task<List<Product>> GetAll();

    Task<Product> GetById(int id);

    Task<Product> GetByName(string name);

    Task<List<Product>> GetExpensiveProducts(decimal minimumPrice);

    Task<Product> Create(CreateProductRequestDto dto);

    Task<Product> UpdateStock(int id, int newStock);

    Task Delete(int id);
}