public class OrderResponseDto
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<OrderItemResponseDto> Items { get; set; } = [];
}