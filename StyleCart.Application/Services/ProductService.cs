using StyleCart.Application.DTOs.Products;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;

namespace StyleCart.Application.Services;

public class ProductService : IProductService
{
    private const string ProductsCacheKey = "products:all";

    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        string? search = null,
        int? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var isUnfilteredRequest =
            string.IsNullOrWhiteSpace(search) &&
            !categoryId.HasValue;

        if (isUnfilteredRequest)
        {
            var cachedProducts =
                await _cacheService.GetAsync<IReadOnlyList<ProductResponse>>(
                    ProductsCacheKey,
                    cancellationToken);

            if (cachedProducts is not null)
            {
                return cachedProducts;
            }
        }

        var products = await _productRepository.GetAllAsync(
            search,
            categoryId,
            cancellationToken);

        var responses = products.Select(MapToResponse).ToList();

        if (isUnfilteredRequest)
        {
            await _cacheService.SetAsync(
                ProductsCacheKey,
                responses,
                TimeSpan.FromMinutes(5),
                cancellationToken);
        }

        return responses;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAvailableAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAvailableAsync(cancellationToken);

        return products.Select(MapToResponse).ToList();
    }

    public Task<IReadOnlyList<ProductCatalogResponse>> GetCatalogAsync(
        CancellationToken cancellationToken = default)
    {
        return _productRepository.GetCatalogAsync(cancellationToken);
    }

    public async Task<ProductResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        return product is null ? null : MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Product name is required.");
        }

        if (request.BasePrice <= 0)
        {
            throw new InvalidOperationException(
                "Product price must be greater than zero.");
        }

        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new InvalidOperationException(
                "Selected category does not exist.");
        }

        if (await _productRepository.ExistsByNameAsync(
                name,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                "A product with this name already exists.");
        }

        var product = new Product
        {
            CategoryId = request.CategoryId,
            Name = name,
            Description = request.Description?.Trim(),
            Brand = request.Brand?.Trim(),
            BasePrice = request.BasePrice
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(ProductsCacheKey, cancellationToken);

        return MapToResponse(product);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Product name is required.");
        }

        if (request.BasePrice <= 0)
        {
            throw new InvalidOperationException(
                "Product price must be greater than zero.");
        }

        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new InvalidOperationException(
                "Selected category does not exist.");
        }

        if (await _productRepository.ExistsByNameAsync(
                name,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "A product with this name already exists.");
        }

        product.CategoryId = request.CategoryId;
        product.Name = name;
        product.Description = request.Description?.Trim();
        product.Brand = request.Brand?.Trim();
        product.BasePrice = request.BasePrice;
        product.IsActive = request.IsActive;

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(ProductsCacheKey, cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(ProductsCacheKey, cancellationToken);

        return true;
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            BasePrice = product.BasePrice,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }
}