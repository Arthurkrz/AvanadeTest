using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Web.Contracts
{
    public interface IProxyService
    {
        Task<IActionResult> ForwardAsync(HttpContext context, string targetService, string? overridePath = null);
    }
}
