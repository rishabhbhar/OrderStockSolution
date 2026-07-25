using ProductService.DTOs;
using ProductService.Entities;
using ProductService.Exceptions;
using ProductService.Repositories;

namespace ProductService.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductServiceImpl> _logger;

        public ProductServiceImpl(IProductRepository repository, ILogger<ProductServiceImpl> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedResultDto<ProductDto>> GetAllAsync(int pageNumber, int pageSize, string? search)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var (items, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize, search);

            _logger.LogInformation("Retrieved {Count} products (page {Page}, size {Size}, total {Total})",
                items.Count, pageNumber, pageSize, totalCount);

            return new PagedResultDto<ProductDto>
            {
                Items = items.Select(MapToDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductDto> GetByIdAsync(Guid productId)
        {
            var product = await _repository.GetByIdAsync(productId)
                ?? throw new NotFoundException($"Product with id '{productId}' was not found.");

            return MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Price = dto.Price,
                StockQty = dto.StockQty,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(product);
            _logger.LogInformation("Created product {ProductId} - {Name}", created.ProductId, created.Name);

            return MapToDto(created);
        }

        public async Task<ProductDto> UpdateAsync(Guid productId, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(productId)
                ?? throw new NotFoundException($"Product with id '{productId}' was not found.");

            product.Name = dto.Name.Trim();
            product.Price = dto.Price;
            product.StockQty = dto.StockQty;
            product.IsActive = dto.IsActive;

            await _repository.UpdateAsync(product);
            _logger.LogInformation("Updated product {ProductId}", productId);

            return MapToDto(product);
        }

        public async Task DeleteAsync(Guid productId)
        {
            var product = await _repository.GetByIdAsync(productId)
                ?? throw new NotFoundException($"Product with id '{productId}' was not found.");

            await _repository.DeleteAsync(product);
            _logger.LogInformation("Deleted (deactivated) product {ProductId}", productId);
        }

        public async Task<StockCheckResponseDto> CheckStockAsync(Guid productId, int quantity)
        {
            var product = await _repository.GetByIdAsync(productId)
                ?? throw new NotFoundException($"Product with id '{productId}' was not found.");

            if (!product.IsActive)
            {
                throw new BadRequestException($"Product with id '{productId}' is not active.");
            }

            return new StockCheckResponseDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                AvailableStock = product.StockQty,
                IsAvailable = product.StockQty >= quantity
            };
        }

        public async Task<ReduceStockResponseDto> ReduceStockAsync(Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new BadRequestException("Quantity must be greater than zero.");
            }

            var product = await _repository.GetByIdAsync(productId)
                ?? throw new NotFoundException($"Product with id '{productId}' was not found.");

            var rowsAffected = await _repository.TryReduceStockAsync(productId, quantity);

            if (rowsAffected == 0)
            {
                throw new InsufficientStockException(
                    $"Insufficient stock for product '{product.Name}'. Requested: {quantity}, Available: {product.StockQty}.");
            }

            var remaining = await _repository.GetCurrentStockAsync(productId);

            _logger.LogInformation("Reduced stock for product {ProductId} by {Quantity}. Remaining: {Remaining}",
                productId, quantity, remaining);

            return new ReduceStockResponseDto
            {
                ProductId = productId,
                RemainingStock = remaining,
                Success = true
            };
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                StockQty = product.StockQty,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
