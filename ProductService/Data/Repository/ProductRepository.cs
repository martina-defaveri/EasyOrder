using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain;

namespace ProductService.Data.Repository;

public class ProductRepository(ProductServiceDbContext dbContext) : IProductRepository
{
    public async Task<Product> CreateProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        var insertedProduct = dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(insertedProduct.Entity).Reference(p => p.Category).LoadAsync();
        return insertedProduct.Entity;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        var foundProduct = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == product.Id);
        ArgumentNullException.ThrowIfNull(foundProduct);

        var updatedProduct = dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(updatedProduct.Entity).Reference(p => p.Category).LoadAsync();
        return updatedProduct.Entity;
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        var foundProduct = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == productId);
        ArgumentNullException.ThrowIfNull(foundProduct);
        dbContext.Products.Remove(foundProduct);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var category = dbContext.Categories.AsNoTracking().FirstOrDefault(x => x.Id == categoryId);
        ArgumentNullException.ThrowIfNull(category);
        var foundProduct = dbContext.Products.Include(x => x.Category).AsNoTracking()
            .Where(x => x.Category.Id == categoryId).ToImmutableList();
        return Task.FromResult<IEnumerable<Product>>(foundProduct);
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId) =>
        await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == productId);

    public Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var foundProduct = dbContext.Products.Include(x => x.Category).AsNoTracking().ToImmutableList();
        return Task.FromResult<IEnumerable<Product>>(foundProduct);
    }
}