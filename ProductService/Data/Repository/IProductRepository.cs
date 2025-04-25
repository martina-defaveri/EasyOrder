using ProductService.Domain;

namespace ProductService.Data.Repository;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
    Task<Product?> GetProductByIdAsync(Guid productId);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(Guid productId);
}