namespace Gateway.API.Web.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            var query = context.Request.Query;
            var user = context.User?.Identity?.Name ?? "Anonymous";

            _logger.LogInformation(
                "Gateway Received Request: {Method} {Path}{Query} by {User}",
                method, path, query, user);

            await _next(context);

            _logger.LogInformation(
                "Gateway Response: {StatusCode} for {Path}{Query}",
                context.Response.StatusCode, path, query);
        }
    }
}