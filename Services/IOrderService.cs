public interface IOrderService
{
    Task<PagedResultDto<OrderResponseDto>> GetAll(
    int page,
    int pageSize);

    Task<OrderResponseDto> GetById(int id);

    Task<OrderResponseDto> Create(CreateOrderRequestDto dto);

    Task<OrderResponseDto> Update(int id, UpdateOrderRequestDto dto);

    Task Delete(int id);
}