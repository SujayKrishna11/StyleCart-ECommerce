using StyleCart.Application.DTOs.ProductImages;

namespace StyleCart.Application.Interfaces;

public interface IProductImageService
{
    Task<IReadOnlyList<ProductImageResponse>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<ProductImageResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductImageResponse> CreateAsync(
        CreateProductImageRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductImageRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}