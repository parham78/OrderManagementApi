using System.ComponentModel.DataAnnotations;

public class CreateMyOrderRequestDto
{
    [Required]
    [MinLength(1)]
    public List<OrderItemRequestDto> Items { get; set; } = [];
}