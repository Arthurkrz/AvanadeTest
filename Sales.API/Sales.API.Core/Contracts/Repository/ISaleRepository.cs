using Sales.API.Core.Entities;
using Sales.API.Core.Enum;

namespace Sales.API.Core.Contracts.Repository
{
    public interface ISaleRepository
    {
        Task<Sale> AddAsync(Sale sale);

        Task<Sale> UpdateStatusAsync(int saleCode, SaleStatus status);

        Task<Sale> GetByIdAsync(Guid id);

        Task<Sale> GetByCodeAsync(int saleCode);

        Task<IEnumerable<Sale>> GetAllAsync();

        Task<IEnumerable<Sale>> GetByBuyerAsync(int buyerCPF);

        Task<IEnumerable<Sale>> GetByProductCodeAsync(int productCode);

        Task<List<int>> GetPendingSalesAsync(TimeSpan maxAge);

        Task<bool> IsSaleExistingByCodeAsync(int saleCode);
    }
}
