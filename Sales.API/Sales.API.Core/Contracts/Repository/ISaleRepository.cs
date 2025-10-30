using Sales.API.Core.Entities;

namespace Sales.API.Core.Contracts.Repository
{
    public interface ISaleRepository
    {
        Sale Add(Sale sale);

        IEnumerable<Sale> GetAll();

        IEnumerable<Sale> GetByBuyer(Guid buyerId);

        IEnumerable<Sale> GetByProductId(Guid productId);

        Sale GetById(Guid id);
    }
}
