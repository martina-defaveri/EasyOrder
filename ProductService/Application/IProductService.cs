using ProductService.Domain;

namespace ProductService.Application;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(Guid id);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
}