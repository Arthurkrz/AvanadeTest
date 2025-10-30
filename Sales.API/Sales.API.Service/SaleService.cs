using Sales.API.Core.Common;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Contracts.Service;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;

namespace Sales.API.Service
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;

        public SaleService(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public Sale Sell(Sale sale)
        {
            
        }

        public IEnumerable<Sale> GetAllSales() => 
            _saleRepository.GetAll();

        public IEnumerable<Sale> GetSalesByBuyerId(Guid buyerId) =>
            _saleRepository.GetByBuyer(buyerId);

        public IEnumerable<Sale> GetSalesByProductId(Guid productId) =>
            _saleRepository.GetByProductId(productId);

        public Sale GetSaleById(Guid id) =>
            _saleRepository.GetById(id);
    }
}
