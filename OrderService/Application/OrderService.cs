using OrderService.Data.Repository;
using OrderService.Domain;

namespace OrderService.Application;

public class OrderService(IOrderRepository repository) : IOrderService
{
    public Task<IEnumerable<Order>> GetAllOrdersAsync() => repository.GetAllOrdersAsync();
    public Task<Order?> GetOrderByIdAsync(Guid id) => repository.GetOrderByIdAsync(id);
    public Task<Order> CreateOrderAsync(Order order) => repository.CreateOrderAsync(order);
    public Task<Order> UpdateOrderAsync(Order order) => repository.UpdateOrderAsync(order);
    public Task<bool> DeleteOrderAsync(Guid id) => repository.DeleteOrderAsync(id);
    
}