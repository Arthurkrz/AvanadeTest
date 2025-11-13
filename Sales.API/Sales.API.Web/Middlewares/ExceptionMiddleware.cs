using Sales.API.Core.Common;

namespace Sales.API.Web.Middlewares
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

            catch (SaleApiException saex)
            {
                _logger.LogWarning()
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception ocurred.");
                await HandleUnexpectedException(context, ex);
            }
        }
    }
}
