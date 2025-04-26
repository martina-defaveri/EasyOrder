namespace OrderService.Domain;

public record OrderProduct(Guid OrderId, Guid ProductId, int NumberOfProducts, decimal ProductPrice)
{
    public decimal ProductsPrice => NumberOfProducts * ProductPrice;
};