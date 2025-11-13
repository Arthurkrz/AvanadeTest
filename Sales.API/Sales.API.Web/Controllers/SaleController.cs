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
            if (saleDTO.ID == Guid.Empty || saleDTO.BuyerID == Guid.Empty ||
                saleDTO.ProductID == Guid.Empty || saleDTO.SellAmount <= 0)
                    return BadRequest(ErrorMessages.INCORRECTFORMAT);

            var sale = new Sale(saleDTO.ID, saleDTO.BuyerID, saleDTO.ProductID,
                                saleDTO.SellAmount, SaleStatus.Pending);

            var createdSale = _saleService.Sell(sale);

            return Ok(new
            {
                createdSale.Result.ID,
                createdSale.Result.ProductID,
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

        public IActionResult GetSalesByBuyerId(Guid buyerId)
        {
            var sales = _saleService.GetSalesByBuyerId(buyerId);

            if (!sales.Any()) return NotFound(ErrorMessages.NOSALESFOUND);

            return Ok(sales);
        }

        public IActionResult GetSalesByProductId(Guid productId)
        {
            var sales = _saleService.GetSalesByProductId(productId);

            if (!sales.Any()) return NotFound(ErrorMessages.NOSALESFOUND);

            return Ok(sales);
        }

        public IActionResult GetById(Guid ID)
        {
            var sale = _saleService.GetSaleById(ID);

            if (sale is null) return NotFound(ErrorMessages.SALENOTFOUND);

            return Ok(sale);
        }   
    }
}
