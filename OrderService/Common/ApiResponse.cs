namespace OrderService.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Request processed successfully.";
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Request processed successfully.")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data };
        }
    }

    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
    }
}
