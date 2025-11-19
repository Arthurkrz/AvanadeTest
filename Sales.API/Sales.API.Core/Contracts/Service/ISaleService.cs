using Sales.API.Core.Entities;

namespace Sales.API.Core.Contracts.Service
{
    public interface ISaleService
    {
        Task<Sale> Sell(Sale sale);

        Sale UpdateSaleStatus(int saleCode, bool success, IList<string> errors);

        Sale GetSaleByCode(int saleCode);

        IEnumerable<Sale> GetAllSales();

        IEnumerable<Sale> GetSalesByBuyerCPF(int buyerCPF);

        IEnumerable<Sale> GetSalesByProductCode(int productCode);
    }
}
