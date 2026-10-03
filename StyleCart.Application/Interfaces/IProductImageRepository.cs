using StyleCart.Domain.Entities;

namespace StyleCart.Application.Interfaces;

public interface IProductImageRepository
{
    Task<IReadOnlyList<ProductImage>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<ProductImage?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductImage image,
        CancellationToken cancellationToken = default);

    void Update(ProductImage image);

    void Remove(ProductImage image);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}