namespace ProductService.Domain;

public sealed record Product(Guid Id, string Name, Guid CategoryId, string Description, decimal Price)
{
    public Category Category { get; set; }
};