using ProductService.Data.Repository;
using ProductService.Domain;

namespace ProductService.Application;

public class ProductService(IProductRepository repository) : IProductService
{
    public Task<IEnumerable<Product>> GetAllProductsAsync() => repository.GetAllProductsAsync();
    public Task<Product?> GetProductByIdAsync(Guid id) => repository.GetProductByIdAsync(id);
    public Task<Product> CreateProductAsync(Product product) => repository.CreateProductAsync(product);
    public Task<Product> UpdateProductAsync(Product product) => repository.UpdateProductAsync(product);
    public Task<bool> DeleteProductAsync(Guid id) => repository.DeleteProductAsync(id);
    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId) => repository.GetProductsByCategoryAsync(categoryId);
    
}