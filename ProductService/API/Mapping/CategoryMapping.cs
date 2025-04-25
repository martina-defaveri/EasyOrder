using AutoMapper;
using ProductService.API.DTO;
using ProductService.Domain;

namespace ProductService.API.Mapping;

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        CreateMap<Category, CategoryToCreate>().ConstructUsing(src =>
            new CategoryToCreate(src.Name));
        CreateMap<Category, CategoryToUpdate>().ConstructUsing(src =>
            new CategoryToUpdate(src.Id, src.Name));
        CreateMap<CategoryToUpdate, Category>().ConstructUsing(src =>
            new Category(src.Id, src.Name));
        CreateMap<CategoryToCreate, Category>().ConstructUsing(src =>
            new Category(Guid.Empty, src.Name));
    }
}