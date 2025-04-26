namespace OrderService.Domain;

public record Order(Guid Id, Guid UserId, Guid AddressId)
{
    public virtual ICollection<OrderProduct> OrderProducts
    {
        get;
        set;
    }
};