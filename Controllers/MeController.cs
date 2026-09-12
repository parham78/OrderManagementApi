using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/me")]
[Authorize(Roles = AppRoles.Customer)]
public class MeController : ControllerBase
{
    private readonly IOrderService _orderService;

    public MeController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetMyOrders(
    int page = 1,
    int pageSize = 10)
    {
        var result =
            await _orderService.GetMyOrders(
                page,
                pageSize);

        return Ok(result);
    }

    [HttpGet("orders/{id}")]
    public async Task<IActionResult> GetMyOrderById(int id)
    {
        var result =
            await _orderService.GetMyOrderById(id);

        return Ok(result);
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateMyOrder(
    CreateMyOrderRequestDto dto)
    {
        var result = await _orderService.CreateMyOrder(dto);

        return Ok(result);
    }

}