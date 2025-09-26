using FluentValidation;
using Stock.API.Core.Common;
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

        public ProductService(IProductRepository productRepository, IValidator<Product> productValidator)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _productValidator = productValidator ?? throw new ArgumentNullException(nameof(productValidator));
        }

        public Product Create(Product product)
        {
            var validationResult = _productValidator.Validate(product);

            if (!validationResult.IsValid) throw new StockApiException(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))),
                ErrorType.BusinessRuleViolation);

            return _productRepository.Create(product);
        }

        public Product UpdateStock(Guid productId, int sellAmount)
        {
            var product = _productRepository.GetById(productId);

            int newAmountInStock = product.AmountInStock - sellAmount;

            return _productRepository.UpdateStock(productId, newAmountInStock);
        }

        public Product UpdateProduct(Guid productId, Product product)
        {
            var validationResult = _productValidator.Validate(product);

            if (!validationResult.IsValid) throw new StockApiException(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))), 
                ErrorType.BusinessRuleViolation);

            return _productRepository.UpdateProduct(productId, product);
        }

        public Product DeleteProduct(Guid productId)
        {
            var deletedProduct = _productRepository.Delete(productId);

            return deletedProduct;
        }

        public IEnumerable<Product> GetAll() => 
            _productRepository.GetAll();
    }
}
