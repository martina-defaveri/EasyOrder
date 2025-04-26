using OrderService.Domain;

namespace OrderService.Application;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderAsync(Order order);
    Task<bool> DeleteOrderAsync(Guid id);
}