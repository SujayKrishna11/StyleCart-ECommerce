using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleCart.Application.DTOs.ProductImages;
using StyleCart.Application.Interfaces;

namespace StyleCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductImagesController : ControllerBase
{
    private readonly IProductImageService _imageService;

    public ProductImagesController(IProductImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductImageResponse>>> GetByProductId(
        [FromQuery] int productId,
        CancellationToken cancellationToken)
    {
        var images = await _imageService.GetByProductIdAsync(
            productId,
            cancellationToken);

        return Ok(images);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductImageResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var image = await _imageService.GetByIdAsync(id, cancellationToken);

        return image is null ? NotFound() : Ok(image);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductImageResponse>> Create(
        CreateProductImageRequest request,
        CancellationToken cancellationToken)
    {
        var image = await _imageService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductImageRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _imageService.UpdateAsync(
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
        var deleted = await _imageService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}