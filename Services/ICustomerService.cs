public interface ICustomerService
{
    Task<List<Customer>> GetAll();

    Task<Customer> GetById(int id);

    Task<Customer> Create(CreateCustomerRequestDto dto);
}