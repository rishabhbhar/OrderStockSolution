namespace OrderService.Common
{
    // Minimal shape used only to read the "message" field out of Product Service's
    // error envelope when a conflict (409 - insufficient stock) response is received.
    public class ErrorResponseLike
    {
        public string? Message { get; set; }
    }
}
