using Sales.API.Core.Common;
using Sales.API.Core.Contracts.Client;
using Sales.API.Core.Contracts.RabbitMQ;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Contracts.Service;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;

namespace Sales.API.Service
{
    public class SaleService : ISaleService
    {
        private readonly IProducerService _producerService;
        private readonly ISaleRepository _saleRepository;

        private readonly IIdentityClient _identityClient;
        private readonly IStockClient _stockClient;

        private readonly Random _random = new();

        public SaleService(IStockClient stockClient, IIdentityClient identityClient, ISaleRepository saleRepository, IProducerService producerService)
        {
            _stockClient = stockClient;
            _identityClient = identityClient;
            _saleRepository = saleRepository;
            _producerService = producerService;
        }

        public async Task<Sale> Sell(Sale sale)
        {
            if (await _stockClient.ProductExistsAsync(sale.ProductCode) &&
                await _identityClient.BuyerExistsAsync(sale.BuyerCPF))
            {
                do sale.SaleCode = _random.Next(1, 10000);
                while (_saleRepository.IsSaleExistingByCode(sale.SaleCode));

                await _producerService.PublishProductSale(
                    sale.SaleCode, sale.ProductCode, sale.SellAmount);

                _saleRepository.Add(sale);
                return _saleRepository.GetById(sale.ID);
            }

            throw new SaleApiException(ErrorMessages.INVALIDSALEREQUEST,
                ErrorType.BusinessRuleViolation);
        }

        public Sale UpdateSaleStatus(int saleCode, bool success, IList<string> errors)
        {
            if (success && errors.Any())
                throw new SaleApiException(ErrorMessages.INVALIDSALESTATUSRESPONSE, 
                    ErrorType.BusinessRuleViolation);

            if (!_saleRepository.IsSaleExistingByCode(saleCode))
                throw new SaleApiException(ErrorMessages.SALENOTFOUND, 
                    ErrorType.NotFound);

            if (!success)
            {
                _saleRepository.UpdateStatus(saleCode, SaleStatus.Rejected);

                throw new SaleApiException(
                    ErrorMessages.SALEFAIL.Replace("{error}", string.Join(", ", errors)),
                    ErrorType.BusinessRuleViolation);
            }

            _saleRepository.UpdateStatus(saleCode, SaleStatus.Completed);
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
