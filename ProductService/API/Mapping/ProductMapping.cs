using AutoMapper;
using ProductService.API.DTO;
using ProductService.Domain;

namespace ProductService.API.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<Product, ProductToCreate>().ConstructUsing(src =>
            new ProductToCreate(src.Name, src.CategoryId, src.Description, src.Price));
        CreateMap<Product, ProductToUpdate>().ConstructUsing(src =>
            new ProductToUpdate(src.Id, src.Name, src.CategoryId, src.Description, src.Price));
        CreateMap<ProductToUpdate, Product>().ConstructUsing(src =>
            new Product(src.Id, src.Name, src.CategoryId, src.Description, src.Price));
        CreateMap<ProductToCreate, Product>().ConstructUsing(src =>
            new Product(Guid.Empty, src.Name, src.CategoryId, src.Description, src.Price));
    }
}