using System.Net;
using System.Text.Json;
using UsersAPI.Models.DTOs.Outgoing;

namespace UsersAPI.Middlewares
{
    public class ExceptionHandingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandingMiddleware> _logger;

        public ExceptionHandingMiddleware(RequestDelegate next, ILogger<ExceptionHandingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, "Internal server error");
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode httpStatusCode, string message)
        {
            _logger.LogError(ex.InnerException?.Message ?? ex.Message);

            HttpResponse response = context.Response;

            response.ContentType = "application/json";
            response.StatusCode = (int)httpStatusCode;

            ErrorDto badResponse = new ErrorDto() {
                Error = message,
                Exception = ex.Message
            };

            string result  = JsonSerializer.Serialize(badResponse);

            await response.WriteAsync(result);
        }
    }
}
