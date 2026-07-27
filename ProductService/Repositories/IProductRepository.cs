using ProductService.Entities;

namespace ProductService.Repositories
{
    public interface IProductRepository
    {
        Task<(List<Product> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? search);
        Task<Product?> GetByIdAsync(Guid productId);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);

        
        Task<int> TryReduceStockAsync(Guid productId, int quantity);

        Task<int> GetCurrentStockAsync(Guid productId);
    }
}
