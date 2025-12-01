using Gateway.API.Web.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("sales")]
    public class SalesController : ControllerBase
    {
        private readonly IProxyService _proxyService;

        public SalesController(IProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        [HttpPost("processSale")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> ProcessSale() =>
            await _proxyService.ForwardAsync(HttpContext, "Sales");

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSales() =>
            await _proxyService.ForwardAsync(HttpContext, "Sales");

        [HttpGet("buyer/{buyerCPF:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSalesByBuyerCPF(int buyerCPF) =>
            await _proxyService.ForwardAsync(HttpContext, "Sales");

        [HttpGet("product/{productCode:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSalesByProductCode(int productCode) =>
            await _proxyService.ForwardAsync(HttpContext, "Sales");

        [HttpGet("sale/{saleCode:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByCode(int saleCode) =>
            await _proxyService.ForwardAsync(HttpContext, "Sales");
    }
}
