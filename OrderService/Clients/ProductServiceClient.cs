using System.Net;
using System.Net.Http.Json;
using OrderService.Common;
using OrderService.DTOs;
using OrderService.Exceptions;

namespace OrderService.Clients
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductServiceClient> _logger;

        public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<StockCheckResponseDto> CheckStockAsync(Guid productId, int quantity)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/{productId}/check-stock?quantity={quantity}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException($"Product with id '{productId}' was not found in Product Service.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Product Service returned {StatusCode} for stock check on {ProductId}: {Body}",
                        response.StatusCode, productId, body);
                    throw new ProductServiceUnavailableException("Unable to verify stock with Product Service at this time.");
                }

                var payload = await response.Content.ReadFromJsonAsync<ApiResponseEnvelope<StockCheckResponseDto>>();

                return payload?.Data
                    ?? throw new ProductServiceUnavailableException("Product Service returned an unexpected response.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to reach Product Service while checking stock for {ProductId}", productId);
                throw new ProductServiceUnavailableException("Product Service is currently unavailable. Please try again later.");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timed out while checking stock for {ProductId}", productId);
                throw new ProductServiceUnavailableException("Product Service request timed out. Please try again later.");
            }
        }

        public async Task<ReduceStockResponseDto> ReduceStockAsync(Guid productId, int quantity)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"api/products/{productId}/reduce-stock",
                    new ReduceStockRequestDto { Quantity = quantity });

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException($"Product with id '{productId}' was not found in Product Service.");
                }

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    var conflictBody = await response.Content.ReadFromJsonAsync<ErrorResponseLike>();
                    throw new InsufficientStockException(conflictBody?.Message
                        ?? "Insufficient stock to fulfil this order.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Product Service returned {StatusCode} for stock reduction on {ProductId}: {Body}",
                        response.StatusCode, productId, body);
                    throw new ProductServiceUnavailableException("Unable to reduce stock with Product Service at this time.");
                }

                var payload = await response.Content.ReadFromJsonAsync<ApiResponseEnvelope<ReduceStockResponseDto>>();

                return payload?.Data
                    ?? throw new ProductServiceUnavailableException("Product Service returned an unexpected response.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to reach Product Service while reducing stock for {ProductId}", productId);
                throw new ProductServiceUnavailableException("Product Service is currently unavailable. Please try again later.");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timed out while reducing stock for {ProductId}", productId);
                throw new ProductServiceUnavailableException("Product Service request timed out. Please try again later.");
            }
        }
    }
}
