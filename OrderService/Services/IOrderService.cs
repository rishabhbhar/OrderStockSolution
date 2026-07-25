using OrderService.DTOs;
using OrderService.Entities;

namespace OrderService.Services
{
    public interface IOrderService
    {
        Task<PagedResultDto<OrderDto>> GetAllAsync(int pageNumber, int pageSize, OrderStatus? status);
        Task<OrderDto> GetByIdAsync(Guid orderId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
    }
}
