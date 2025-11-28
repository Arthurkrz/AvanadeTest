namespace Gateway.API.Web.Middlewares
{
    public class RequestValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestValidationMiddleware> _logger;

        public RequestValidationMiddleware(RequestDelegate next, ILogger<RequestValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if ((context.Request.Method == "POST" || context.Request.Method == "PUT") &&
                context.Request.ContentType == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Content-Type header required.");
                return;
            }

            await _next(context);
        }
    }
}
