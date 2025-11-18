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

        public Product Create(Product product)
        {
            var validationResult = _productValidator.Validate(product);

            if (!validationResult.IsValid) throw new StockApiException(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))),
                ErrorType.BusinessRuleViolation);

            do product.Code = _random.Next(1, 10000);
            while (_productRepository.GetByCode(product.Code) is not null);

            return _productRepository.Create(product);
        }

        public void UpdateStock(int saleCode, int productCode, int sellAmount)
        {
            var product = _productRepository.GetByCode(productCode);

            if (product is null) 
            {
                _producerService.PublishSaleProcessed(
                saleCode, productCode, [ErrorMessages.PRODUCTNOTFOUND]);

                return;
            }

            if (sellAmount > product!.AmountInStock) _producerService.PublishSaleProcessed(
                saleCode, productCode, [ErrorMessages.NOTENOUGHSTOCK]);

            int newAmountInStock = product.AmountInStock - sellAmount;

            _productRepository.UpdateStock(productCode, newAmountInStock);
            _producerService.PublishSaleProcessed(saleCode, productCode);
        }

        public Product UpdateProduct(int productCode, Product product)
        {
            var validationResult = _productValidator.Validate(product);

            if (_productRepository.GetByCode(productCode) is null)
                throw new StockApiException(ErrorMessages.PRODUCTNOTFOUND, ErrorType.NotFound);

            if (!validationResult.IsValid) throw new StockApiException(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))), 
                ErrorType.BusinessRuleViolation);

            return _productRepository.UpdateProduct(productCode, product);
        }

        public Product DeleteProduct(int productCode)
        {
            if (_productRepository.GetByCode(productCode) is null)
                throw new StockApiException(ErrorMessages.PRODUCTNOTFOUND, ErrorType.NotFound);

            var deletedProduct = _productRepository.Delete(productCode);

            return deletedProduct;
        }

        public Product GetByCode(int ProductCode) =>
            _productRepository.GetByCode(ProductCode);

        public IEnumerable<Product> GetAll() => 
            _productRepository.GetAll();
    }
}
