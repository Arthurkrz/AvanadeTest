using Sales.API.Core.Entities;

namespace Sales.API.Core.Contracts.Service
{
    public interface ISaleService
    {
        Task<Sale> SellAsync(Sale sale);

        Task<Sale> UpdateSaleStatusAsync(int saleCode, bool success, IList<string> errors);

        Task<Sale> GetSaleByCodeAsync(int saleCode);

        Task<IEnumerable<Sale>> GetAllSalesAsync();

        Task<IEnumerable<Sale>> GetSalesByBuyerCPFAsync(int buyerCPF);

        Task<IEnumerable<Sale>> GetSalesByProductCodeAsync(int productCode);
    }
}
