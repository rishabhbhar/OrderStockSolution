namespace ProductService.DTOs
{
    
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
