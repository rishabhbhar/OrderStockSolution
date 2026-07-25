namespace OrderService.DTOs
{
    // Mirrors the response contracts exposed by Product Service's inter-service endpoints.
    // Order Service only ever talks to Product Service through HTTP - never directly to
    // its database - and these DTOs are the shape of that contract.
    public class ApiResponseEnvelope<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public class StockCheckResponseDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class ReduceStockRequestDto
    {
        public int Quantity { get; set; }
    }

    public class ReduceStockResponseDto
    {
        public Guid ProductId { get; set; }
        public int RemainingStock { get; set; }
        public bool Success { get; set; }
    }
}
