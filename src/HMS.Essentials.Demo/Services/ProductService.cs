using HMS.Essentials.Domain.Repositories;
using HMS.Essentials.Domain.UnitOfWork;
using HMS.Essentials.Entities;

namespace HMS.Essentials.Services;

/// <summary>
/// Service demonstrating repository pattern usage for product management.
/// </summary>
public class ProductService
{
    private readonly IRepository<Product, int> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IRepository<Product, int> productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Product> CreateProductAsync(string name, decimal price, int stock, string category)
    {
        var product = new Product
        {
            Name = name,
            Price = price,
            Stock = stock,
            Category = category,
            Description = $"Sample product: {name}",
            IsActive = true
        };

        return await _productRepository.InsertAsync(product, autoSave: true);
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _productRepository.GetListAsync(p => p.Category == category);
    }

    public async Task<List<Product>> GetExpensiveProductsAsync(decimal minPrice)
    {
        return await _productRepository.GetListAsync(p => p.Price >= minPrice && p.IsActive);
    }

    public async Task<List<Product>> GetLowStockProductsAsync(int threshold)
    {
        return await _productRepository.GetListAsync(p => p.Stock <= threshold);
    }

    public async Task<bool> UpdateProductPriceAsync(int productId, decimal newPrice)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return false;

        product.Price = newPrice;
        await _productRepository.UpdateAsync(product, autoSave: true);
        return true;
    }

    public async Task<bool> AdjustStockAsync(int productId, int quantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return false;

        product.Stock += quantity;
        await _productRepository.UpdateAsync(product, autoSave: true);
        return true;
    }

    public async Task TransferStockAsync(int fromProductId, int toProductId, int quantity)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var fromProduct = await _productRepository.GetByIdAsync(fromProductId);
            var toProduct = await _productRepository.GetByIdAsync(toProductId);

            if (fromProduct == null || toProduct == null)
                throw new InvalidOperationException("One or both products not found");

            if (fromProduct.Stock < quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {fromProduct.Stock}, Required: {quantity}");

            fromProduct.Stock -= quantity;
            toProduct.Stock += quantity;

            await _productRepository.UpdateAsync(fromProduct);
            await _productRepository.UpdateAsync(toProduct);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            Console.WriteLine($"✓ Transferred {quantity} units from '{fromProduct.Name}' to '{toProduct.Name}'");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            Console.WriteLine($"✗ Stock transfer failed: {ex.Message}");
            throw;
        }
    }

    public async Task<(List<Product> Products, long TotalCount)> GetPagedProductsAsync(int page, int pageSize)
    {
        var skipCount = (page - 1) * pageSize;
        var products = await _productRepository.GetPagedListAsync(skipCount, pageSize);
        var totalCount = await _productRepository.CountAsync();

        return (products, totalCount);
    }

    public async Task DeleteProductAsync(int productId)
    {
        await _productRepository.DeleteAsync(productId, autoSave: true);
    }

    public async Task DeleteInactiveProductsAsync()
    {
        await _productRepository.DeleteManyAsync(p => !p.IsActive, autoSave: true);
    }
}
