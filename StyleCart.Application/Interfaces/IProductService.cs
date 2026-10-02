using StyleCart.Application.DTOs.Products;

namespace StyleCart.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        string? search = null,
        int? categoryId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductResponse>> GetAvailableAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductCatalogResponse>> GetCatalogAsync(
        CancellationToken cancellationToken = default);

    Task<ProductResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}