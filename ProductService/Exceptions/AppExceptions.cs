namespace ProductService.Exceptions
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
        public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound) { }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest) { }
    }

    public class InsufficientStockException : AppException
    {
        public InsufficientStockException(string message) : base(message, StatusCodes.Status409Conflict) { }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message, StatusCodes.Status409Conflict) { }
    }

    internal static class StatusCodes
    {
        public const int Status400BadRequest = 400;
        public const int Status404NotFound = 404;
        public const int Status409Conflict = 409;
    }
}
