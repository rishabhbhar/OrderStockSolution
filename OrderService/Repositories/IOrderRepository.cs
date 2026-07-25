using OrderService.Entities;

namespace OrderService.Repositories
{
    public interface IOrderRepository
    {
        Task<(List<Order> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, OrderStatus? status);
        Task<Order?> GetByIdAsync(Guid orderId);
        Task<Order> AddAsync(Order order);
        Task UpdateAsync(Order order);
    }
}
