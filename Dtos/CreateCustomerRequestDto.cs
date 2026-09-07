using System.ComponentModel.DataAnnotations;

public class CreateCustomerRequestDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}