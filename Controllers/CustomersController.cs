using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAll();

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerService.GetById(id);

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCustomerRequestDto dto)
    {
        var customer = await _customerService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id },
            customer);
    }
}