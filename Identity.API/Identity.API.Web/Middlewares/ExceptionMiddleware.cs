using Identity.API.Core.Common;
using Identity.API.Core.Enum;
using System.Net;
using System.Text.Json;

namespace Identity.API.Web.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }

            catch (IdentityApiException iaex)
            {
                _logger.LogWarning(iaex, "An IdentityApiException occurred: {Message}", iaex.Message);
                await HandleIdentityApiException(context, iaex);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleUnexpectedException(context, ex);
            }
        }

        public async Task HandleIdentityApiException(HttpContext context, IdentityApiException iaex)
        {
            var (statusCode, title) = MapErrorType(iaex.ErrorType);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                error = iaex.Error,
                title,
                message = iaex.Message,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        public async Task HandleUnexpectedException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                error = "InternalError",
                title = "Internal Server Error",
                message = "Unexpected error occurred.",
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static (int StatusCode, string Title) MapErrorType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
                ErrorType.InternalError => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
                ErrorType.DatabaseError => (StatusCodes.Status500InternalServerError, "Database Error"),
                ErrorType.IntegrationError => (StatusCodes.Status503ServiceUnavailable, "Integration Error"),
                ErrorType.BusinessRuleViolation => (StatusCodes.Status422UnprocessableEntity, "Business Rule Violation"),
                _ => (StatusCodes.Status500InternalServerError, "Undefined Error")
            };
    }
}
