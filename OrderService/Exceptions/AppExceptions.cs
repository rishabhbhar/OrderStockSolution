namespace OrderService.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, 404) { }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message, 400) { }
    }

    public class InsufficientStockException : AppException
    {
        public InsufficientStockException(string message) : base(message, 409) { }
    }

    // Raised when the downstream Product Service cannot be reached at all
    // (network failure, timeout, DNS, service down, etc.).
    public class ProductServiceUnavailableException : AppException
    {
        public ProductServiceUnavailableException(string message) : base(message, 503) { }
    }
}
