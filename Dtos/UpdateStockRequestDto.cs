using System.ComponentModel.DataAnnotations;

public class UpdateStockRequestDto
{
    [Required(ErrorMessage = "NewStock is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
    public int? NewStock { get; set; }
}