using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain;

namespace OrderService.Data.Repository;

public class OrderRepository(OrderServiceDbContext dbContext) : IOrderRepository
{
    public async Task<Order> CreateOrderAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        var insertedOrder = dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();
        return insertedOrder.Entity;
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var foundOrder = await dbContext.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(x => x.Id == order.Id);

        ArgumentNullException.ThrowIfNull(foundOrder);

        var existingProducts = foundOrder.OrderProducts.ToList();

        foreach (var newProduct in order.OrderProducts)
        {
            if (existingProducts.All(ep => ep.ProductId != newProduct.ProductId))
            {
                dbContext.OrderProducts.Add(newProduct);
            }
        }

        foreach (var existingProduct in existingProducts.Where(existingProduct =>
                     order.OrderProducts.All(np => np.ProductId != existingProduct.ProductId)))
        {
            dbContext.OrderProducts.Remove(existingProduct);
        }

        await dbContext.SaveChangesAsync();

        var reloadedOrder = await dbContext.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        return reloadedOrder ?? throw new InvalidOperationException();
    }


    public async Task<bool> DeleteOrderAsync(Guid orderId)
    {
        var foundOrder = await dbContext.Orders.AsNoTracking().Include(order => order.OrderProducts)
            .FirstOrDefaultAsync(x => x.Id == orderId);
        ArgumentNullException.ThrowIfNull(foundOrder);
        dbContext.Orders.Remove(foundOrder);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId) =>
        await dbContext.Orders.Include(x => x.OrderProducts).AsNoTracking().FirstOrDefaultAsync(x => x.Id == orderId);

    public Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        var foundOrder = dbContext.Orders.Include(x => x.OrderProducts).AsNoTracking().ToImmutableList();
        return Task.FromResult<IEnumerable<Order>>(foundOrder);
    }
}