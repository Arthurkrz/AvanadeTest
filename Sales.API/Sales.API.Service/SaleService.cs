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

        public async Task<Sale> SellAsync(Sale sale)
        {
            if (await _stockClient.ProductExistsAsync(sale.ProductCode) &&
                await _identityClient.BuyerExistsAsync(sale.BuyerCPF))
            {
                do sale.SaleCode = _random.Next(1, 10000);
                while (await _saleRepository.IsSaleExistingByCodeAsync(sale.SaleCode));

                await _producerService.PublishProductSale(
                    sale.SaleCode, sale.ProductCode, sale.SellAmount);

                await _saleRepository.AddAsync(sale);
                return await _saleRepository.GetByIdAsync(sale.ID);
            }

            throw new SaleApiException(ErrorMessages.INVALIDSALEREQUEST,
                ErrorType.BusinessRuleViolation);
        }

        public async Task<Sale> UpdateSaleStatusAsync(int saleCode, bool success, IList<string> errors)
        {
            if (success && errors.Any())
                throw new SaleApiException(ErrorMessages.INVALIDSALESTATUSRESPONSE, 
                    ErrorType.BusinessRuleViolation);

            if (!await _saleRepository.IsSaleExistingByCodeAsync(saleCode))
                throw new SaleApiException(ErrorMessages.SALENOTFOUND, 
                    ErrorType.NotFound);

            if (!success)
            {
                await _saleRepository.UpdateStatusAsync(saleCode, SaleStatus.Rejected);

                throw new SaleApiException(
                    ErrorMessages.SALEFAIL.Replace("{error}", string.Join(", ", errors)),
                    ErrorType.BusinessRuleViolation);
            }

            await _saleRepository.UpdateStatusAsync(saleCode, SaleStatus.Completed);
            return await _saleRepository.GetByCodeAsync(saleCode);
        }

        public async Task<Sale> GetSaleByCodeAsync(int saleCode) => 
            await _saleRepository.GetByCodeAsync(saleCode);

        public async Task<IEnumerable<Sale>> GetAllSalesAsync() => 
            await _saleRepository.GetAllAsync();

        public async Task<IEnumerable<Sale>> GetSalesByBuyerCPFAsync(int buyerCPF) =>
            await _saleRepository.GetByBuyerAsync(buyerCPF);

        public async Task<IEnumerable<Sale>> GetSalesByProductCodeAsync(int productCode) =>
            await _saleRepository.GetByProductCodeAsync(productCode);
    }
}
