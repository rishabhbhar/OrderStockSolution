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

        
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }
}
