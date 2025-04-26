using OrderService.Domain;

namespace OrderService.Data.Repository;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(Guid orderId);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderAsync(Order order);
    Task<bool> DeleteOrderAsync(Guid orderId);
}