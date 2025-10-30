using Sales.API.Core.Entities;

namespace Sales.API.Core.Contracts.Service
{
    public interface ISaleService
    {
        Sale Sell(Sale sale);

        IEnumerable<Sale> GetAllSales();

        IEnumerable<Sale> GetSalesByBuyerId(Guid buyerId);

        IEnumerable<Sale> GetSalesByProductId(Guid productId);

        Sale GetSaleById(Guid id);
    }
}
