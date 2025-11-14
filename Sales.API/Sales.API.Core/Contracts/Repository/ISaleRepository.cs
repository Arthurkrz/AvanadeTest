using Sales.API.Core.Entities;
using Sales.API.Core.Enum;

namespace Sales.API.Core.Contracts.Repository
{
    public interface ISaleRepository
    {
        Sale Add(Sale sale);

        Sale UpdateStatus(int saleCode, SaleStatus status);

        Sale GetById(Guid id);

        Sale GetByCode(int saleCode);

        IEnumerable<Sale> GetAll();

        IEnumerable<Sale> GetByBuyer(int buyerCPF);

        IEnumerable<Sale> GetByProductCode(int productCode);

        bool IsSaleExistingByCode(int saleCode);
    }
}
