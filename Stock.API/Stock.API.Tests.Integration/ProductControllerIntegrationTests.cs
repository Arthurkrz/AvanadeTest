using FluentValidation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Validators;
using Stock.API.IOC;
using Stock.API.Tests.Integration.Utilities;
using Stock.API.Web.DTOs;
using Stock.API.Web.Validators;
using StockAPI.Controllers;
using FluentValidation.Results;

namespace Stock.API.Tests.Integration
{
    public class ProductControllerIntegrationTests
    {
        private readonly ProductController _sut;
        private readonly ProductTestTableManager _productTestTableManager;
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

            services.AddValidatorsFromAssemblyContaining<ProductDTOValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductValidator>();

            services.InjectRepositories(config);
            services.InjectServices();
            services.InjectValidators();

            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<Context>();
            _productDTOValidator = _serviceProvider.GetRequiredService<IValidator<ProductDTO>>();
            _productValidator = _serviceProvider.GetRequiredService<IValidator<Product>>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _productService = _serviceProvider.GetRequiredService<IProductService>();

            _productTestTableManager = new ProductTestTableManager(_context, _productRepository);
            _sut = new ProductController(_productService, _productDTOValidator);
        }

        [Fact]
        public void Create_ShouldCreateAndInsertNewProduct()
        {
            // Arrange
            _productTestTableManager.Cleanup();

            var productDTO = new ProductDTO()
            {
                Name = "ProductName",
                Description = "ProductDescription",
                Price = 10.5M,
                AmountInStock = 10
            };

            // Act
            var result = _sut.Create(productDTO);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<Product>(ok.Value);

            Assert.Equal("ProductName", actual.Name);
            Assert.Equal("ProductDescription", actual.Description);
            Assert.Equal(10.5M, actual.Price);
            Assert.Equal(10, actual.AmountInStock);

            var products = _productRepository.GetAll();
            Assert.Single(products);

            Product? addedProduct = _productRepository.GetById(products.First().ID);
            Assert.Equal(addedProduct.Name, actual.Name);
            Assert.Equal(addedProduct.Price, actual.Price);
            Assert.Equal(addedProduct.AmountInStock, actual.AmountInStock);
        }

        [Theory]
        [MemberData(nameof(GetInvalidProductDTOs))]
        public void Create_ShouldReturnBadRequest_WhenInvalidProductDTO(ProductDTO productDTO, IList<ValidationFailure> expectedError)
        {
            // Arrange
            _productTestTableManager.Cleanup();

            // Act
            var result = _sut.Create(productDTO);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var actual = Assert.IsAssignableFrom<IList<ValidationFailure>>(badRequest.Value);

            var actualAnonymous = actual.Select(e => new { e.PropertyName, e.ErrorMessage });
            var expectedAnonymous = expectedError.Select(e => new { e.PropertyName, e.ErrorMessage });

            actualAnonymous.Should().BeEquivalentTo(expectedAnonymous);
        }

        [Fact]
        public void Sell_ShouldUpdateProductStock()
        {
            // Arrange
            _productTestTableManager.Cleanup();
            _productTestTableManager.InsertProduct();
            var productId = _productRepository.GetAll().First().ID;

            // Act
            var result = _sut.Sell(productId, 5);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<Product>(ok.Value!);

            Assert.Equal(productId, actual.ID);
            Assert.Equal("Name0", actual.Name);
            Assert.Equal(10M, actual.Price);
            Assert.Equal(5, actual.AmountInStock);

            var updatedProduct = _productRepository.GetById(productId);
            Assert.Equal(5, updatedProduct.AmountInStock);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", 1)]
        [InlineData("3f0a1a47-fc3b-4b2e-9c41-6b2f7b7d9a55", 0)]
        [InlineData("3f0a1a47-fc3b-4b2e-9c41-6b2f7b7d9a55", -1)]
        public void Sell_ShouldReturnBadRequest_WhenInvalidRequest(Guid id, int sellAmount)
        {
            // Arrange
            _productTestTableManager.Cleanup();

            // Act
            var result = _sut.Sell(id, sellAmount);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.INCORRECTFORMAT, badRequest.Value);
        }

        [Fact]
        public void Update_ShouldUpdateProduct()
        {
            // Arrange
            _productTestTableManager.Cleanup();
            _productTestTableManager.InsertProduct();

            var productDTO = new ProductDTO()
            {
                Name = "UpdatedProductName",
                Description = "UpdatedProductDescription",
                Price = 20.5M,
                AmountInStock = 15
            };

            var productId = _productRepository.GetAll().First().ID;

            // Act
            var result = _sut.Update(productId, productDTO);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<Product>(ok.Value!);

            Assert.Equal("UpdatedProductName", actual.Name);
            Assert.Equal("UpdatedProductDescription", actual.Description);
            Assert.Equal(20.5M, actual.Price);
            Assert.Equal(15, actual.AmountInStock);

            var updatedProduct = _productRepository.GetById(productId);
            Assert.Equal("UpdatedProductName", updatedProduct.Name);
            Assert.Equal("UpdatedProductDescription", updatedProduct.Description);
            Assert.Equal(20.5M, updatedProduct.Price);
            Assert.Equal(15, updatedProduct.AmountInStock);
        }

        [Theory]
        [MemberData(nameof(GetInvalidProductDTOs))]
        public void Update_ShouldReturnBadRequestWhenInvalidProductDTO(ProductDTO productDTO, IList<ValidationFailure> expectedError)
        {
            // Arrange
            _productTestTableManager.Cleanup();

            // Act
            var result = _sut.Update(Guid.NewGuid(), productDTO);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var actual = Assert.IsAssignableFrom<IList<ValidationFailure>>(badRequest.Value);

            var actualAnonymous = actual.Select(e => new { e.PropertyName, e.ErrorMessage });
            var expectedAnonymous = expectedError.Select(e => new { e.PropertyName, e.ErrorMessage });

            actualAnonymous.Should().BeEquivalentTo(expectedAnonymous);
        }

        [Fact]
        public void Update_ShouldReturnBadRequest_WhenInvalidId()
        {
            // Arrange
            _productTestTableManager.Cleanup();

            var productDTO = new ProductDTO()
            {
                Name = "ProductName",
                Description = "ProductDescription",
                Price = 10.5M,
                AmountInStock = 10
            };

            // Act
            var result = _sut.Update(Guid.Empty, productDTO);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.INCORRECTFORMAT, badRequest.Value);
        }

        [Fact]
        public void Delete_ShouldDeleteProduct()
        {
            // Arrange
            _productTestTableManager.Cleanup();
            _productTestTableManager.InsertProduct();
            var productId = _productRepository.GetAll().First().ID;

            // Act
            var result = _sut.Delete(productId);

            // Assert
            Assert.Null(_productRepository.GetById(productId));
            var ok = Assert.IsType<OkObjectResult>(result);
            var anon = ok.Value!;

            var actual = new
            {
                ID = (Guid)anon.GetType().GetProperty("ID")!.GetValue(anon)!,
                Name = (string)anon.GetType().GetProperty("Name")!.GetValue(anon)!
            };

            Assert.Equal(productId, actual.ID);
            Assert.Equal("Name0", actual.Name);
        }

        [Fact]
        public void Delete_ShouldReturnBadRequest_WhenInvalidId()
        {
            // Arrange
            _productTestTableManager.Cleanup();

            // Act
            var result = _sut.Delete(Guid.Empty);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.INCORRECTFORMAT, badRequest.Value);
        }

        [Fact]
        public void GetAll_ShouldReturnAllProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product("Name0", "Description0", 10, 10),

                new Product("Name1", "Description1", 10, 10),

                new Product("Name2", "Description2", 10, 10)
            };

            _productTestTableManager.Cleanup();
            _productTestTableManager.InsertProduct(3);

            // Act
            var result = _sut.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);

            products.Should().BeEquivalentTo(expectedProducts, options => options
                .Excluding(p => p.ID)
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(5)))
                .WhenTypeIs<DateTime>());
        }

        [Fact]
        public void GetAll_ShouldReturnBadRequest_WhenNoProducts()
        {
            // Arrange
            _productTestTableManager.Cleanup();

            // Act
            var result = _sut.GetAll();

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.NOPRODUCTSFOUND, badRequest.Value);
        }

        public static IEnumerable<object[]> GetInvalidProductDTOs()
        {
            yield return new object[]
            {
                new ProductDTO()
                {
                    Name = "",
                    Description = "Description",
                    Price = 10,
                    AmountInStock = 10
                },

                new List<ValidationFailure>
                {
                    new ValidationFailure("Name", "Product name must not be null or empty")
                }
            };

            yield return new object[]
            {
                new ProductDTO()
                {
                    Name = null,
                    Description = "Description",
                    Price = 10,
                    AmountInStock = 10
                },

                new List<ValidationFailure>
                {
                    new ValidationFailure("Name", "Product name must not be null or empty")
                }
            };

            yield return new object[]
            {
                new ProductDTO()
                {
                    Name = "Name",
                    Description = "",
                    Price = 10,
                    AmountInStock = 10
                },

                new List<ValidationFailure>
                {
                    new ValidationFailure("Description", "Product description must not be null or empty")
                }
            };

            yield return new object[]
            {
                new ProductDTO()
                {
                    Name = "Name",
                    Description = null,
                    Price = 10,
                    AmountInStock = 10
                },

                new List<ValidationFailure>
                {
                    new ValidationFailure("Description", "Product description must not be null or empty")
                }
            };

            yield return new object[]
            {
                new ProductDTO()
                {
                    Name = "Name",
                    Description = "Description",
                    Price = 0,
                    AmountInStock = 10
                },

                new List<ValidationFailure>
                {
                    new ValidationFailure("Price", "Product price must not be zero or negative")
                }
            };

            yield return new object[]
            {
                new ProductDTO()
                {
                    Name = "Name",
                    Description = "Description",
                    Price = -1,
                    AmountInStock = 10
                },

                new List<ValidationFailure>
                {
                    new ValidationFailure("Price", "Product price must not be zero or negative")
                }
            };

            yield return new object[]
            {
                new ProductDTO()
                {
                    Name = "Name",
                    Description = "Description",
                    Price = 10,
                    AmountInStock = -1
                },

                new List<ValidationFailure>
                {
                    new ValidationFailure("AmountInStock", "Product amount must not be negative")
                }
            };
        }
    }
}
