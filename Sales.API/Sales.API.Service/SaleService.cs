using Sales.API.Core.Common;
using Sales.API.Core.Contracts.Client;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Contracts.Service;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;

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
            if (await _stockClient.ProductExistsAsync(sale.ProductCode) &&
                await _identityClient.BuyerExistsAsync(sale.BuyerCPF))
            {
                // Message prod
            }
            
            _saleRepository.Add(sale);
            return _saleRepository.GetById(sale.ID);
        }

        public Sale UpdateSaleStatus(int saleCode, SaleStatus status)
        {
            if (status == SaleStatus.Pending)
                throw new SaleApiException(ErrorMessages.INVALIDSTATUS, ErrorType.BusinessRuleViolation);

            if (!_saleRepository.IsSaleExistingByCode(saleCode))
                throw new SaleApiException(ErrorMessages.SALENOTFOUND, ErrorType.NotFound);

            _saleRepository.UpdateStatus(saleCode, status);
            return _saleRepository.GetByCode(saleCode);
        }

        public Sale GetSaleByCode(int saleCode) => 
            _saleRepository.GetByCode(saleCode);

        public IEnumerable<Sale> GetAllSales() => 
            _saleRepository.GetAll();

        public IEnumerable<Sale> GetSalesByBuyerCPF(int buyerCPF) =>
            _saleRepository.GetByBuyer(buyerCPF);

        public IEnumerable<Sale> GetSalesByProductCode(int productCode) =>
            _saleRepository.GetByProductCode(productCode);
    }
}
