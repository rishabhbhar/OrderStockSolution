using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "ProductId is required.")]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
    }
}
