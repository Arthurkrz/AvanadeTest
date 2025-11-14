using Sales.API.Core.Entities;
using Sales.API.Core.Enum;

namespace Sales.API.Core.Contracts.Service
{
    public interface ISaleService
    {
        Task<Sale> Sell(Sale sale);

        Sale UpdateSaleStatus(int saleCode, SaleStatus status);

        Sale GetSaleByCode(int saleCode);

        IEnumerable<Sale> GetAllSales();

        IEnumerable<Sale> GetSalesByBuyerCPF(int buyerCPF);

        IEnumerable<Sale> GetSalesByProductCode(int productCode);
    }
}
