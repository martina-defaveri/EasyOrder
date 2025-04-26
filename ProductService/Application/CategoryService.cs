using ProductService.Data.Repository;
using ProductService.Domain;

namespace ProductService.Application;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public Task<IEnumerable<Category>> GetAllCategoriesAsync() => repository.GetAllCategoriesAsync();
    public Task<Category?> GetCategoryByIdAsync(Guid id) => repository.GetCategoryByIdAsync(id);
    public Task<Category> CreateCategoryAsync(Category category) => repository.AddCategoryAsync(category);
    public Task<Category> UpdateCategoryAsync(Category category) => repository.UpdateCategoryAsync(category);
    public Task<bool> DeleteCategoryAsync(Guid id) => repository.DeleteCategoryAsync(id);
}