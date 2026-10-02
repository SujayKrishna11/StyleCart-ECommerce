using StyleCart.Application.DTOs.Products;
using StyleCart.Domain.Entities;

namespace StyleCart.Application.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(
        string? search = null,
        int? categoryId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAvailableAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductCatalogResponse>> GetCatalogAsync(
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default);

    void Update(Product product);

    void Remove(Product product);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}