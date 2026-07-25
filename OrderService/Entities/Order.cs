namespace OrderService.Entities
{
    public enum OrderStatus
    {
        CREATED,
        PAID,
        CANCELLED
    }

    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.CREATED;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Denormalized snapshot fields captured at order time - useful for display/history
        // without ever needing to query the Product database directly.
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }
}
