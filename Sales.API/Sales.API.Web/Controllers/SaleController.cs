using Microsoft.AspNetCore.Mvc;
using Sales.API.Core.Common;
using Sales.API.Core.Contracts.Service;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;
using Sales.API.Web.DTOs;

namespace Sales.API.Web.Controllers
{
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        public IActionResult ProcessSale(SaleDTO saleDTO)
        {
            if (saleDTO.BuyerCPF <= 0 || saleDTO.ProductCode <= 0 || saleDTO.SellAmount <= 0)
                    return BadRequest(ErrorMessages.INCORRECTFORMAT);

            var sale = new Sale(saleDTO.BuyerCPF, saleDTO.ProductCode,
                                saleDTO.SellAmount, SaleStatus.Pending);

            var createdSale = _saleService.Sell(sale);

            return Ok(new
            {
                createdSale.Result.ProductCode,
                createdSale.Result.SellAmount,
                Status = createdSale.Result.Status.ToString()
            });
        }

        public IActionResult GetAllSales()
        {
            var sales = _saleService.GetAllSales();

            if (!sales.Any()) return NotFound(ErrorMessages.NOSALESFOUND);

            return Ok(sales);
        }

        public IActionResult GetSalesByBuyerId(int buyerCPF)
        {
            var sales = _saleService.GetSalesByBuyerCPF(buyerCPF);

            if (!sales.Any()) return NotFound(ErrorMessages.NOSALESFOUND);

            return Ok(sales);
        }

        public IActionResult GetSalesByProductId(int productCode)
        {
            var sales = _saleService.GetSalesByProductCode(productCode);

            if (!sales.Any()) return NotFound(ErrorMessages.NOSALESFOUND);

            return Ok(sales);
        }

        public IActionResult GetByCode(int saleCode)
        {
            var sale = _saleService.GetSaleByCode(saleCode);

            if (sale is null) return NotFound(ErrorMessages.SALENOTFOUND);

            return Ok(sale);
        }   
    }
}
