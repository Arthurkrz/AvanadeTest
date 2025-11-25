using FluentValidation;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.RabbitMQ;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Enum;

namespace Stock.API.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<Product> _productValidator;
        private readonly IProducerService _producerService;
        private readonly Random _random = new();

        public ProductService(IProductRepository productRepository, IValidator<Product> productValidator, IProducerService producerService)
        {
            _productRepository = productRepository;
            _productValidator = productValidator;
            _producerService = producerService;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            var validationResult = _productValidator.Validate(product);

            if (!validationResult.IsValid) throw new StockApiException(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))),
                ErrorType.BusinessRuleViolation);

            do product.Code = _random.Next(1, 10000);
            while (await _productRepository.IsExistingByCodeAsync(product.Code));

            return await _productRepository.CreateAsync(product);
        }

        public async Task UpdateStockAsync(int saleCode, int productCode, int sellAmount)
        {
            var product = await _productRepository.GetByCodeAsync(productCode);

            if (product is null) 
            {
                await _producerService.PublishSaleProcessedAsync(
                saleCode, productCode, [ErrorMessages.PRODUCTNOTFOUND]);

                return;
            }

            if (sellAmount > product!.AmountInStock) 
                await _producerService.PublishSaleProcessedAsync(
                saleCode, productCode, [ErrorMessages.NOTENOUGHSTOCK]);

            int newAmountInStock = product.AmountInStock - sellAmount;

            await _productRepository.UpdateStockAsync(productCode, newAmountInStock);
            await _producerService.PublishSaleProcessedAsync(saleCode, productCode);
        }

        public async Task<Product> UpdateProductAsync(int productCode, Product product)
        {
            var validationResult = _productValidator.Validate(product);

            if (!await _productRepository.IsExistingByCodeAsync(productCode))
                throw new StockApiException(ErrorMessages.PRODUCTNOTFOUND, ErrorType.NotFound);

            if (!validationResult.IsValid) throw new StockApiException(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))), 
                ErrorType.BusinessRuleViolation);

            return await _productRepository.UpdateProductAsync(productCode, product);
        }

        public async Task<Product> DeleteProductAsync(int productCode)
        {
            if (await _productRepository.GetByCodeAsync(productCode) is null)
                throw new StockApiException(ErrorMessages.PRODUCTNOTFOUND, ErrorType.NotFound);

            var deletedProduct = await _productRepository.DeleteAsync(productCode);

            return deletedProduct;
        }

        public async Task<Product> GetByCodeAsync(int ProductCode) =>
            await _productRepository.GetByCodeAsync(ProductCode);

        public async Task<IEnumerable<Product>> GetAllAsync() => 
            await _productRepository.GetAllAsync();
    }
}
