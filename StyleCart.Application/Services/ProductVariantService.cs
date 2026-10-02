using StyleCart.Application.DTOs.ProductVariants;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;

namespace StyleCart.Application.Services;

public class ProductVariantService : IProductVariantService
{
    private readonly IProductVariantRepository _variantRepository;
    private readonly IProductRepository _productRepository;

    public ProductVariantService(
        IProductVariantRepository variantRepository,
        IProductRepository productRepository)
    {
        _variantRepository = variantRepository;
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductVariantResponse>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var variants = await _variantRepository.GetByProductIdAsync(
            productId,
            cancellationToken);

        return variants.Select(MapToResponse).ToList();
    }

    public async Task<ProductVariantResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var variant = await _variantRepository.GetByIdAsync(id, cancellationToken);

        return variant is null ? null : MapToResponse(variant);
    }

    public async Task<ProductVariantResponse> CreateAsync(
        CreateProductVariantRequest request,
        CancellationToken cancellationToken = default)
    {
        var size = request.Size.Trim();
        var color = request.Color.Trim();
        var sku = request.SKU.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(size) ||
            string.IsNullOrWhiteSpace(color) ||
            string.IsNullOrWhiteSpace(sku))
        {
            throw new InvalidOperationException(
                "Size, color, and SKU are required.");
        }

        if (request.Price <= 0)
        {
            throw new InvalidOperationException(
                "Variant price must be greater than zero.");
        }

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            throw new InvalidOperationException("Selected product does not exist.");
        }

        if (await _variantRepository.ExistsBySkuAsync(
                sku,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("This SKU already exists.");
        }

        var variant = new ProductVariant
        {
            ProductId = request.ProductId,
            Size = size,
            Color = color,
            SKU = sku,
            Price = request.Price
        };

        await _variantRepository.AddAsync(variant, cancellationToken);
        await _variantRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(variant);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductVariantRequest request,
        CancellationToken cancellationToken = default)
    {
        var variant = await _variantRepository.GetByIdAsync(id, cancellationToken);

        if (variant is null)
        {
            return false;
        }

        var size = request.Size.Trim();
        var color = request.Color.Trim();
        var sku = request.SKU.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(size) ||
            string.IsNullOrWhiteSpace(color) ||
            string.IsNullOrWhiteSpace(sku))
        {
            throw new InvalidOperationException(
                "Size, color, and SKU are required.");
        }

        if (request.Price <= 0)
        {
            throw new InvalidOperationException(
                "Variant price must be greater than zero.");
        }

        if (await _variantRepository.ExistsBySkuAsync(
                sku,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException("This SKU already exists.");
        }

        variant.Size = size;
        variant.Color = color;
        variant.SKU = sku;
        variant.Price = request.Price;
        variant.IsActive = request.IsActive;

        _variantRepository.Update(variant);
        await _variantRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var variant = await _variantRepository.GetByIdAsync(id, cancellationToken);

        if (variant is null)
        {
            return false;
        }

        _variantRepository.Remove(variant);
        await _variantRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static ProductVariantResponse MapToResponse(ProductVariant variant)
    {
        var isInStock = variant.Inventory is not null &&
                        variant.Inventory.QuantityInStock >
                        variant.Inventory.ReservedQuantity;

        return new ProductVariantResponse
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            Size = variant.Size,
            Color = variant.Color,
            SKU = variant.SKU,
            Price = variant.Price,
            IsActive = variant.IsActive,
            IsInStock = isInStock
        };
    }
}