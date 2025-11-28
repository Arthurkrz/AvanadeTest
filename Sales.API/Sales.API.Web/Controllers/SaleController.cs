using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Core.Common;
using Sales.API.Core.Contracts.Service;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;
using Sales.API.Web.DTOs;

namespace Sales.API.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/sales")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpPost("processSale")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> ProcessSale([FromBody] SaleDTO saleDTO)
        {
            if (saleDTO.BuyerCPF <= 0 || saleDTO.ProductCode <= 0 || saleDTO.SellAmount <= 0)
                    return BadRequest(ErrorMessages.INCORRECTFORMAT);

            var sale = new Sale(saleDTO.BuyerCPF, saleDTO.ProductCode,
                                saleDTO.SellAmount, SaleStatus.Pending);

            var createdSale = await _saleService.SellAsync(sale);

            return Ok(createdSale);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSales()
        {
            var sales = await _saleService.GetAllSalesAsync();

            if (!sales.Any()) return NotFound(ErrorMessages.NOSALESFOUND);

            return Ok(sales);
        }

        [HttpGet("buyer/{buyerCPF:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSalesByBuyerCPF(int buyerCPF)
        {
            var sales = await _saleService.GetSalesByBuyerCPFAsync(buyerCPF);

            if (!sales.Any()) return NotFound(ErrorMessages.NOSALESFOUND);

            return Ok(sales);
        }

        [HttpGet("product/{productCode:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSalesByProductCode(int productCode)
        {
            var sales = await _saleService.GetSalesByProductCodeAsync(productCode);

            if (!sales.Any()) return NotFound(ErrorMessages.NOSALESFOUND);

            return Ok(sales);
        }

        [HttpGet("sale/{saleCode:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByCode(int saleCode)
        {
            var sale = await _saleService.GetSaleByCodeAsync(saleCode);

            if (sale is null) return NotFound(ErrorMessages.SALENOTFOUND);

            return Ok(sale);
        }   
    }
}
