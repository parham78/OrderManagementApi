using System.ComponentModel.DataAnnotations;

public class CreateOrderRequestDto
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemRequestDto> Items { get; set; } = [];
}