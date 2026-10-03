using Microsoft.EntityFrameworkCore;
using StyleCart.Application.DTOs.Products;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;
using StyleCart.Infrastructure.Data;

namespace StyleCart.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(
        string? search = null,
        int? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(product => product.Name.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAvailableAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product =>
                product.IsActive &&
                _dbContext.ProductVariants.Any(variant =>
                    variant.ProductId == product.Id &&
                    variant.IsActive))
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductCatalogResponse>> GetCatalogAsync(
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product =>
                product.IsActive &&
                _dbContext.ProductVariants.Any(variant =>
                    variant.ProductId == product.Id &&
                    variant.IsActive))
            .OrderBy(product => product.Name)
            .Select(product => new ProductCatalogResponse
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                BasePrice = product.BasePrice,
                IsActive = product.IsActive,

                IsInStock = _dbContext.ProductVariants.Any(variant =>
                    variant.ProductId == product.Id &&
                    variant.IsActive &&
                    _dbContext.Inventories.Any(inventory =>
                        inventory.ProductVariantId == variant.Id &&
                        inventory.QuantityInStock >
                        inventory.ReservedQuantity)),

                ImageUrl = _dbContext.ProductImages
                    .Where(image => image.ProductId == product.Id)
                    .OrderBy(image => image.DisplayOrder)
                    .ThenBy(image => image.Id)
                    .Select(image => image.ImageUrl)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.AnyAsync(
            product => product.Name == name &&
                       (!excludeId.HasValue || product.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        _dbContext.Products.Update(product);
    }

    public void Remove(Product product)
    {
        _dbContext.Products.Remove(product);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}