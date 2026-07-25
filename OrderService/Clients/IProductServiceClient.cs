using OrderService.DTOs;

namespace OrderService.Clients
{
    // Sole point of contact with Product Service. Order Service MUST go through this
    // interface for anything product/stock related and must never query the Product
    // database directly.
    public interface IProductServiceClient
    {
        Task<StockCheckResponseDto> CheckStockAsync(Guid productId, int quantity);
        Task<ReduceStockResponseDto> ReduceStockAsync(Guid productId, int quantity);
    }
}
