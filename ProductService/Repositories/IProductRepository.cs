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

        // Atomically decrements stock only if enough stock is available.
        // Returns the number of rows affected (0 = insufficient stock or product not found).
        Task<int> TryReduceStockAsync(Guid productId, int quantity);

        Task<int> GetCurrentStockAsync(Guid productId);
    }
}
