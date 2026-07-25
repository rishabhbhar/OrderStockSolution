using System.Net;
using System.Text.Json;
using OrderService.Common;
using OrderService.Exceptions;

namespace OrderService.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            var response = new ErrorResponse
            {
                Success = false,
                TraceId = traceId
            };

            switch (exception)
            {
                case AppException appEx:
                    response.StatusCode = appEx.StatusCode;
                    response.Message = appEx.Message;
                    _logger.LogWarning(exception, "Handled application exception. TraceId: {TraceId}", traceId);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An unexpected error occurred while processing your request.";
                    response.Details = _env.IsDevelopment() ? exception.ToString() : null;
                    _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.StatusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
