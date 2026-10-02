using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleCart.Application.DTOs.Products;
using StyleCart.Application.Interfaces;

namespace StyleCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IProductBulkImportService _productBulkImportService;

    public ProductsController(
        IProductService productService,
        IProductBulkImportService productBulkImportService)
    {
        _productService = productService;
        _productBulkImportService = productBulkImportService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(
            search,
            categoryId,
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAvailable(
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetAvailableAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("catalog")]
    public async Task<ActionResult<IReadOnlyList<ProductCatalogResponse>>> GetCatalog(
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetCatalogAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);

        return product is null ? NotFound() : Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("bulk-import")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BulkProductImportResult>> BulkImport(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "Please upload a non-empty Excel file."
            });
        }

        if (!string.Equals(
                Path.GetExtension(file.FileName),
                ".xlsx",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Only .xlsx Excel files are supported."
            });
        }

        await using var fileStream = file.OpenReadStream();

        var result = await _productBulkImportService.ImportAsync(
            fileStream,
            cancellationToken);

        return result.IsSuccessful
            ? Ok(result)
            : BadRequest(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _productService.UpdateAsync(
            id,
            request,
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _productService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}