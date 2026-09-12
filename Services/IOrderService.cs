public interface IOrderService
{
    Task<PagedResultDto<OrderResponseDto>> GetAll(
    int page,
    int pageSize);
    Task<PagedResultDto<OrderResponseDto>> GetMyOrders(
    int page,
    int pageSize);
    Task<OrderResponseDto> GetMyOrderById(int id);
    Task<OrderResponseDto> CreateMyOrder(
    CreateMyOrderRequestDto dto);

    Task<OrderResponseDto> GetById(int id);

    Task<OrderResponseDto> Create(CreateOrderRequestDto dto);

    Task<OrderResponseDto> Update(int id, UpdateOrderRequestDto dto);

    Task Delete(int id);
}