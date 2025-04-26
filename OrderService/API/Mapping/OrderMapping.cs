using AutoMapper;
using OrderService.API.DTO;
using OrderService.Domain;

namespace OrderService.API.Mapping;

public class OrderMapping : Profile
{
    public OrderMapping()
    {
        CreateMap<Order, OrderToCreate>().ConstructUsing(src =>
            new OrderToCreate(src.UserId, src.AddressId, src.OrderProducts.Select(op =>
                new OrderProductToCreate(
                    op.ProductId,
                    op.NumberOfProducts,
                    op.ProductPrice)
            ).ToList()));
        CreateMap<Order, OrderToUpdate>().ConstructUsing(src =>
            new OrderToUpdate(src.Id, src.UserId, src.AddressId, src.OrderProducts.Select(op =>
                new OrderProductToUpdate(
                    op.OrderId,
                    op.ProductId,
                    op.NumberOfProducts,
                    op.ProductPrice)
            ).ToList()));
        CreateMap<OrderToUpdate, Order>().ConstructUsing(src =>
            new Order(src.Id, src.UserId, src.AddressId)
            {
                OrderProducts = src.OrderProducts.Select(op =>
                    new OrderProduct(
                        op.OrderId,
                        op.ProductId,
                        op.NumberOfProducts,
                        op.ProductPrice)
                ).ToList()
            });
        CreateMap<OrderToCreate, Order>().ConstructUsing(src =>
            new Order(Guid.Empty, src.UserId, src.AddressId)
            {
                OrderProducts = src.OrderProducts.Select(op =>
                    new OrderProduct(
                        Guid.Empty, 
                        op.ProductId,
                        op.NumberOfProducts,
                        op.ProductPrice)
                ).ToList()
            });
        CreateMap<OrderProduct, OrderProductToCreate>().ConstructUsing(src =>
            new OrderProductToCreate(src.ProductId, src.NumberOfProducts, src.ProductPrice));
        CreateMap<OrderProduct, OrderProductToUpdate>().ConstructUsing(src =>
            new OrderProductToUpdate(src.OrderId, src.ProductId, src.NumberOfProducts, src.ProductPrice));
        CreateMap<OrderProductToCreate, OrderProduct>().ConstructUsing(src =>
            new OrderProduct(Guid.Empty, src.ProductId, src.NumberOfProducts, src.ProductPrice));
        CreateMap<OrderProductToUpdate, OrderProduct>().ConstructUsing(src =>
            new OrderProduct(src.OrderId, src.ProductId, src.NumberOfProducts, src.ProductPrice));
    }
}