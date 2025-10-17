using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.IOC;
using Stock.API.Web.DTOs;
using StockAPI.Controllers;

namespace Stock.API.Tests.Integration
{
    public class ProductControllerIntegrationTests
    {
        private readonly ProductController _sut;
        private readonly IServiceProvider _serviceProvider;
        private readonly Context _context;

        private readonly IValidator<ProductDTO> _productDTOValidator;
        private readonly IValidator<Product> _productValidator;

        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;

        public ProductControllerIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ServiceCollection services = new();

            services.InjectRepositories(config);
            services.InjectServices();
            services.InjectValidators();

            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<Context>();

            _productDTOValidator = _serviceProvider.GetRequiredService<IValidator<ProductDTO>>();
            _productValidator = _serviceProvider.GetRequiredService<IValidator<Product>>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _productService = _serviceProvider.GetRequiredService<IProductService>();

            _sut = new ProductController(_productService, _productDTOValidator);
        }

        [Fact]
        public void Create_ShouldCreateAndInsertNewProduct()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Create_ShouldReturnBadRequest_WhenInvalidProductDTO()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Sell_ShouldUpdateProductStock()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Sell_ShouldReturnBadRequest_WhenInvalidRequest()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Update_ShouldUpdateProduct()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Update_ShouldReturnBadRequestWhenInvalidProductDTO()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Update_ShouldReturnBadRequest_WhenInvalidId()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Delete_ShouldDeleteProduct()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Delete_ShouldReturnBadRequest_WhenInvalidId()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void GetAll_ShouldReturnAllProducts()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void GetAll_ShouldReturnBadRequest_WhenNoProducts()
        {
            throw new NotImplementedException();
        }
    }
}
