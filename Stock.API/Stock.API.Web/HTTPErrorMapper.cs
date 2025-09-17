using Microsoft.AspNetCore.Mvc;
using Stock.API.Core.Enum;

namespace Stock.API.Web
{
    public static class HTTPErrorMapper
    {
        public static IActionResult Map(ErrorType errorType, IList<string> errors) => errorType switch
        {
            ErrorType.Conflict => new ConflictObjectResult(new ProblemDetails()
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = string.Join("; ", errors)
            }),
            
            ErrorType.NotFound => new NotFoundObjectResult(new ProblemDetails()
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = string.Join("; ", errors)
            }),
            
            ErrorType.InternalError => new ObjectResult(new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = string.Join("; ", errors)
            }),

            ErrorType.DatabaseError => new ObjectResult(new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Database Error",
                Detail = string.Join("; ", errors)
            }),

            ErrorType.IntegrationError => new ObjectResult(new ProblemDetails()
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "Integration Error",
                Detail = string.Join("; ", errors)
            }),

            ErrorType.BusinessRuleViolation => new ObjectResult(new ProblemDetails()
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Business Rule Violation",
                Detail = string.Join("; ", errors)
            }),
            
            _ => new ObjectResult(new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Undefined Error",
                Detail = string.Join("; ", errors)
            })
        };
    }
}
