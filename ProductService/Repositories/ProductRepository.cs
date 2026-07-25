using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Entities;

namespace ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ProductDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(List<Product> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? search)
        {
            var query = _context.Products.AsNoTracking().Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Product?> GetByIdAsync(Guid productId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            // Soft delete keeps history and avoids breaking existing order references.
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<int> TryReduceStockAsync(Guid productId, int quantity)
        {
            // A single conditional UPDATE guarantees atomicity at the database level:
            // the row is only decremented if there is currently enough stock, avoiding
            // race conditions between the stock-check and the stock-reduction steps.
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Products
                SET StockQty = StockQty - {quantity}, UpdatedAt = SYSUTCDATETIME()
                WHERE ProductId = {productId} AND StockQty >= {quantity} AND IsActive = 1");

            if (rowsAffected == 0)
            {
                _logger.LogWarning("Stock reduction failed for ProductId {ProductId}, requested quantity {Quantity}", productId, quantity);
            }

            return rowsAffected;
        }

        public async Task<int> GetCurrentStockAsync(Guid productId)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == productId);
            return product?.StockQty ?? -1;
        }
    }
}
