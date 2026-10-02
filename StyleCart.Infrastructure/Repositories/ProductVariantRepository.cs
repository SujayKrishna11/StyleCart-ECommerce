using Microsoft.EntityFrameworkCore;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;
using StyleCart.Infrastructure.Data;

namespace StyleCart.Infrastructure.Repositories;

public class ProductVariantRepository : IProductVariantRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductVariantRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductVariant>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductVariants
            .AsNoTracking()
            .Include(variant => variant.Inventory)
            .Where(variant => variant.ProductId == productId)
            .OrderBy(variant => variant.Color)
            .ThenBy(variant => variant.Size)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductVariant?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductVariants
            .Include(variant => variant.Inventory)
            .FirstOrDefaultAsync(variant => variant.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(
        string sku,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductVariants.AnyAsync(
            variant => variant.SKU == sku &&
                       (!excludeId.HasValue || variant.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        ProductVariant productVariant,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.ProductVariants.AddAsync(productVariant, cancellationToken);
    }

    public void Update(ProductVariant productVariant)
    {
        _dbContext.ProductVariants.Update(productVariant);
    }

    public void Remove(ProductVariant productVariant)
    {
        _dbContext.ProductVariants.Remove(productVariant);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}