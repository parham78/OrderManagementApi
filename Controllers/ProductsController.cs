using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAll();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetById(id);

        if (product is null)
        {
            return NotFound(new { message = "Product not found" });
        }

        return Ok(product);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var product = await _productService.GetByName(name);

        if (product is null)
        {
            return NotFound(new { message = "Product not found" });
        }

        return Ok(product);
    }

    [HttpGet("expensive")]
    public async Task<IActionResult> GetExpensiveProducts(
        decimal minimumPrice)
    {
        var products =
            await _productService.GetExpensiveProducts(minimumPrice);

        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        CreateProductRequestDto dto)
    {
        var product = await _productService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(
        int id,
        int newStock)
    {
        var product =
            await _productService.UpdateStock(id, newStock);

        if (product is null)
        {
            return NotFound(
                new { message = "Product not found" });
        }

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool result =
            await _productService.Delete(id);

        if (!result)
        {
            return NotFound(
                new { message = "Product not found" });
        }

        return NoContent();
    }
}