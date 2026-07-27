using OrderService.DTOs;

namespace OrderService.Clients
{
    
    public interface IProductServiceClient
    {
        Task<StockCheckResponseDto> CheckStockAsync(Guid productId, int quantity);
        Task<ReduceStockResponseDto> ReduceStockAsync(Guid productId, int quantity);
    }
}
