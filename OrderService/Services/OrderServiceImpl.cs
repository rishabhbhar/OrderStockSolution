using OrderService.Clients;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Exceptions;
using OrderService.Repositories;

namespace OrderService.Services
{
    public class OrderServiceImpl : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductServiceClient _productServiceClient;
        private readonly ILogger<OrderServiceImpl> _logger;

        public OrderServiceImpl(
            IOrderRepository orderRepository,
            IProductServiceClient productServiceClient,
            ILogger<OrderServiceImpl> logger)
        {
            _orderRepository = orderRepository;
            _productServiceClient = productServiceClient;
            _logger = logger;
        }

        public async Task<PagedResultDto<OrderDto>> GetAllAsync(int pageNumber, int pageSize, OrderStatus? status)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var (items, totalCount) = await _orderRepository.GetAllAsync(pageNumber, pageSize, status);

            return new PagedResultDto<OrderDto>
            {
                Items = items.Select(MapToDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<OrderDto> GetByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException($"Order with id '{orderId}' was not found.");

            return MapToDto(order);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            if (dto.Quantity <= 0)
            {
                throw new BadRequestException("Order quantity must be greater than zero.");
            }

            _logger.LogInformation("Creating order for ProductId {ProductId}, Quantity {Quantity}", dto.ProductId, dto.Quantity);

            // Step 1: Ask Product Service whether enough stock exists. This is an early,
            // fast-fail check - the authoritative, race-safe check happens in Step 2 where
            // Product Service performs an atomic conditional UPDATE.
            var stock = await _productServiceClient.CheckStockAsync(dto.ProductId, dto.Quantity);

            if (!stock.IsAvailable)
            {
                _logger.LogWarning(
                    "Order rejected: insufficient stock for ProductId {ProductId}. Requested {Requested}, Available {Available}",
                    dto.ProductId, dto.Quantity, stock.AvailableStock);

                throw new InsufficientStockException(
                    $"Cannot place order: requested quantity ({dto.Quantity}) exceeds available stock ({stock.AvailableStock}) for product '{stock.Name}'.");
            }

            // Step 2: Perform the atomic stock reduction on Product Service. Product Service
            // guarantees this is race-safe (single conditional UPDATE), so this is the true
            // gatekeeper - if two orders race for the last unit, only one reduction succeeds.
            var reduceResult = await _productServiceClient.ReduceStockAsync(dto.ProductId, dto.Quantity);

            if (!reduceResult.Success)
            {
                throw new InsufficientStockException(
                    $"Cannot place order: stock for product '{stock.Name}' became insufficient before the order could be confirmed.");
            }

            // Step 3: Only after stock has been confirmed and reduced do we persist the order.
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                ProductId = dto.ProductId,
                ProductName = stock.Name,
                UnitPrice = stock.Price,
                Quantity = dto.Quantity,
                OrderStatus = OrderStatus.CREATED,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var created = await _orderRepository.AddAsync(order);
                _logger.LogInformation("Order {OrderId} created successfully for ProductId {ProductId}", created.OrderId, dto.ProductId);
                return MapToDto(created);
            }
            catch (Exception ex)
            {
                // Stock has already been decremented on Product Service at this point but the
                // order record failed to persist locally. In a production system this gap
                // should be closed with a saga / outbox pattern that compensates by restoring
                // stock. It is logged here as a critical error so it can be reconciled manually
                // or picked up by a compensating background job.
                _logger.LogCritical(ex,
                    "CRITICAL: Stock was reduced for ProductId {ProductId} (Quantity {Quantity}) but order persistence failed. Manual reconciliation required.",
                    dto.ProductId, dto.Quantity);
                throw;
            }
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                ProductName = order.ProductName,
                UnitPrice = order.UnitPrice,
                Quantity = order.Quantity,
                OrderStatus = order.OrderStatus.ToString(),
                CreatedAt = order.CreatedAt
            };
        }
    }
}
