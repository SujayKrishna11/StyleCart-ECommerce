using Microsoft.EntityFrameworkCore;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;
using StyleCart.Infrastructure.Data;

namespace StyleCart.Infrastructure.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductImageRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductImage>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductImages
            .AsNoTracking()
            .Where(image => image.ProductId == productId)
            .OrderBy(image => image.DisplayOrder)
            .ThenBy(image => image.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductImage?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductImages
            .FirstOrDefaultAsync(image => image.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        ProductImage image,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.ProductImages.AddAsync(image, cancellationToken);
    }

    public void Update(ProductImage image)
    {
        _dbContext.ProductImages.Update(image);
    }

    public void Remove(ProductImage image)
    {
        _dbContext.ProductImages.Remove(image);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}