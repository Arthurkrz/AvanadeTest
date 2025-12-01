using Gateway.API.Web.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Web.Controllers
{
    [ApiController]
    [Route("stock")]
    public class StockController : ControllerBase
    {
        private readonly IProxyService _proxyService;

        public StockController(IProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create() =>
            await _proxyService.ForwardAsync(HttpContext, "Stock");

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{productCode:int}")]
        public async Task<IActionResult> Update(int productCode) =>
            await _proxyService.ForwardAsync(HttpContext, "Stock");

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{productCode:int}")] 
        public async Task<IActionResult> Delete(int productCode) => 
            await _proxyService.ForwardAsync(HttpContext, "Stock");

        [Authorize(Roles = "Admin,Buyer")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() =>
            await _proxyService.ForwardAsync(HttpContext, "Stock");

        [Authorize(Roles = "Admin,Buyer")]
        [HttpGet("product/{productCode:int}")]
        public async Task<IActionResult> GetByCode(int productCode) =>
            await _proxyService.ForwardAsync(HttpContext, "Stock");
    }
}
