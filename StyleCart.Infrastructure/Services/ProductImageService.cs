using StyleCart.Application.DTOs.ProductImages;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;

namespace StyleCart.Application.Services;

public class ProductImageService : IProductImageService
{
    private readonly IProductImageRepository _imageRepository;
    private readonly IProductRepository _productRepository;

    public ProductImageService(
        IProductImageRepository imageRepository,
        IProductRepository productRepository)
    {
        _imageRepository = imageRepository;
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductImageResponse>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var images = await _imageRepository.GetByProductIdAsync(
            productId,
            cancellationToken);

        return images.Select(MapToResponse).ToList();
    }

    public async Task<ProductImageResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var image = await _imageRepository.GetByIdAsync(id, cancellationToken);

        return image is null ? null : MapToResponse(image);
    }

    public async Task<ProductImageResponse> CreateAsync(
        CreateProductImageRequest request,
        CancellationToken cancellationToken = default)
    {
        var imageUrl = ValidateImageUrl(request.ImageUrl);

        if (request.DisplayOrder < 0)
        {
            throw new InvalidOperationException(
                "Display order cannot be negative.");
        }

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            throw new InvalidOperationException(
                "Selected product does not exist.");
        }

        var image = new ProductImage
        {
            ProductId = request.ProductId,
            ImageUrl = imageUrl,
            AltText = request.AltText?.Trim(),
            DisplayOrder = request.DisplayOrder
        };

        await _imageRepository.AddAsync(image, cancellationToken);
        await _imageRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(image);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductImageRequest request,
        CancellationToken cancellationToken = default)
    {
        var image = await _imageRepository.GetByIdAsync(id, cancellationToken);

        if (image is null)
        {
            return false;
        }

        if (request.DisplayOrder < 0)
        {
            throw new InvalidOperationException(
                "Display order cannot be negative.");
        }

        image.ImageUrl = ValidateImageUrl(request.ImageUrl);
        image.AltText = request.AltText?.Trim();
        image.DisplayOrder = request.DisplayOrder;

        _imageRepository.Update(image);
        await _imageRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var image = await _imageRepository.GetByIdAsync(id, cancellationToken);

        if (image is null)
        {
            return false;
        }

        _imageRepository.Remove(image);
        await _imageRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string ValidateImageUrl(string imageUrl)
    {
        var trimmedUrl = imageUrl.Trim();

        if (trimmedUrl.StartsWith(
                "/product-images/",
                StringComparison.OrdinalIgnoreCase))
        {
            return trimmedUrl;
        }

        var isValidUrl = Uri.TryCreate(
            trimmedUrl,
            UriKind.Absolute,
            out var uri);

        if (!isValidUrl ||
            (uri.Scheme != Uri.UriSchemeHttp &&
             uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                "Image URL must be a valid HTTP or HTTPS URL, " +
                "or a local /product-images/ path.");
        }

        return trimmedUrl;
    }

    private static ProductImageResponse MapToResponse(ProductImage image)
    {
        return new ProductImageResponse
        {
            Id = image.Id,
            ProductId = image.ProductId,
            ImageUrl = image.ImageUrl,
            AltText = image.AltText,
            DisplayOrder = image.DisplayOrder
        };
    }
}