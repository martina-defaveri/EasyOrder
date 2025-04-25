using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using ProductService.Domain;

namespace ProductService.Data.Repository;

public class CategoryRepository(ProductServiceDbContext context) : ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        => await context.Categories.AsNoTracking().ToListAsync();

    public async Task<Category?> GetCategoryByIdAsync(Guid id)
        => await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Category> AddCategoryAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        var categoryFound = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Name == category.Name);
        if (categoryFound != null) throw new KeyException("Category already exists.");
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        var categoryFound =
            await context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == category.Name && c.Id != category.Id);
        if (categoryFound != null) throw new KeyException("Category already exists.");
        context.Categories.Update(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Category.Id == id);
        if (product != null) throw new KeyException("There are products in this category.");
        var category = await context.Categories.FindAsync(id);
        ArgumentNullException.ThrowIfNull(category);

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }
}