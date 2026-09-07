using System.ComponentModel.DataAnnotations;

public class UpdateOrderRequestDto
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemRequestDto> Items { get; set; } = [];
}