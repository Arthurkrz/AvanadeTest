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

        public Response<Product> Create(Product product)
        {
            var validationResult = _productValidator.Validate(product);

            if (!validationResult.IsValid) 
                return Response<Product>.Ko((ErrorType)6, validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return _productRepository.Create(product);
        }

        public Response<Product> UpdateStock(Guid productId, int sellAmount)
        {
            var response = _productRepository.GetById(productId);

            if (!response.Success) return response;

            if (response.Value!.AmountInStock < sellAmount)
                return Response<Product>.Ko((ErrorType)6, new List<string> { "Not enough items in stock." });

            int newAmountInStock = response.Value!.AmountInStock - sellAmount;

            return _productRepository.UpdateStock(productId, newAmountInStock);
        }

        public Response<Product> UpdateProduct(Guid productId, Product product)
        {
            var validationResult = _productValidator.Validate(product);

            if (!validationResult.IsValid)
                return Response<Product>.Ko((ErrorType)6, validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return _productRepository.UpdateProduct(productId, product);
        }

        public Response<Product> DeleteProduct(Guid productId)
        {
            var response = _productRepository.Delete(productId);
            if (!response.Success) return response;

            return Response<Product>.Ok(response.Value!);
        }

        public IEnumerable<Product> GetAll() => 
            _productRepository.GetAll();
    }
}
