using Sales.API.Core.Contracts.Client;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Contracts.Service;
using Sales.API.Core.Entities;

namespace Sales.API.Service
{
    public class SaleService : ISaleService
    {
        private readonly IStockClient _stockClient;
        private readonly IIdentityClient _identityClient;
        private readonly ISaleRepository _saleRepository;

        public SaleService(IStockClient stockClient, IIdentityClient identityClient, ISaleRepository saleRepository)
        {
            _stockClient = stockClient;
            _identityClient = identityClient;
            _saleRepository = saleRepository;
        }

        public async Task<Sale> Sell(Sale sale)
        {
            if (await _stockClient.ProductExistsAsync(sale.ProductID) &&
                await _identityClient.BuyerExistsAsync(sale.BuyerID))
            {
                // Message prod
            }

            _saleRepository.Add(sale);
            return _saleRepository.GetById(sale.ID);
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
