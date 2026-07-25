namespace ProductService.DTOs
{
    // Used by Order Service to check whether stock is available before placing an order.
    public class StockCheckResponseDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }
        public bool IsAvailable { get; set; }
    }

    // Used by Order Service to atomically reduce stock after a successful order.
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
