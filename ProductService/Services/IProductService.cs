using ProductService.DTOs;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductDto>> GetAllAsync(int pageNumber, int pageSize, string? search);
        Task<ProductDto> GetByIdAsync(Guid productId);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(Guid productId, UpdateProductDto dto);
        Task DeleteAsync(Guid productId);

        Task<StockCheckResponseDto> CheckStockAsync(Guid productId, int quantity);
        Task<ReduceStockResponseDto> ReduceStockAsync(Guid productId, int quantity);
    }
}
