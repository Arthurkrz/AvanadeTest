using Gateway.API.Web.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Web.Controllers
{
    [ApiController]
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        private readonly IProxyService _proxyService;

        public IdentityController(IProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        [AllowAnonymous]
        [HttpPost("admin/register")]
        public async Task<IActionResult> AdminRegister() =>
            await _proxyService.ForwardAsync(HttpContext, "Identity");

        [AllowAnonymous]
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin() =>
            await _proxyService.ForwardAsync(HttpContext, "Identity");

        [AllowAnonymous]
        [HttpPost("buyer/register")]
        public async Task<IActionResult> BuyerRegister() =>
            await _proxyService.ForwardAsync(HttpContext, "Identity");

        [AllowAnonymous]
        [HttpPost("buyer/login")]
        public async Task<IActionResult> BuyerLogin() =>
        await _proxyService.ForwardAsync(HttpContext, "Identity");
    }
}