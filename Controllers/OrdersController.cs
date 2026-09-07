using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
    int page = 1,
    int pageSize = 10)
    {
        var orders = await _orderService.GetAll(
            page,
            pageSize);

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetById(id);

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequestDto dto)
    {
        var createdOrder =
            await _orderService.Create(dto);

        return CreatedAtAction(
            nameof(GetOrderById),
            new { id = createdOrder.Id },
            createdOrder
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(
        int id,
        UpdateOrderRequestDto dto)
    {
        var order =
            await _orderService.Update(id, dto);

        return Ok(order);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        await _orderService.Delete(id);

        return NoContent();
    }
}