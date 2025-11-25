using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.IOC;
using Stock.API.Tests.Integration.Utilities;
using Stock.API.Web.DTOs;
using Stock.API.Web.Validators;
using StockAPI.Controllers;

namespace Stock.API.Tests.Integration
{
    public class ProductControllerIntegrationTests
    {
        private readonly ProductController _sut;
        private readonly ProductTestTableManager _productTestTableManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly Context _context;

        private readonly IValidator<ProductDTO> _productDTOValidator;

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

            services.InjectRepositories(config);
            services.InjectRabbitMQ(config);
            services.InjectServices();
            services.InjectValidators();

            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<Context>();
            _productDTOValidator = _serviceProvider.GetRequiredService<IValidator<ProductDTO>>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _productService = _serviceProvider.GetRequiredService<IProductService>();

            _productTestTableManager = new ProductTestTableManager(_context, _productRepository);
            _sut = new ProductController(_productService, _productDTOValidator);
        }

        [Fact]
        public async Task Create_ShouldCreateAndInsertNewProduct()
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
            var result = await _sut.Create(productDTO);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<Product>(ok.Value);

            Assert.Equal("ProductName", actual.Name);
            Assert.Equal("ProductDescription", actual.Description);
            Assert.Equal(10.5M, actual.Price);
            Assert.Equal(10, actual.AmountInStock);

            var products = await _productRepository.GetAllAsync();
            Assert.Single(products);

            Product? addedProduct = await _productRepository.GetByIdAsync(products.First().ID);
            Assert.Equal(addedProduct.Name, actual.Name);
            Assert.Equal(addedProduct.Price, actual.Price);
            Assert.Equal(addedProduct.AmountInStock, actual.AmountInStock);
        }

        [Theory]
        [MemberData(nameof(GetInvalidProductDTOs))]
        public async Task Create_ShouldReturnBadRequest_WhenInvalidProductDTO(ProductDTO productDTO, IList<ValidationFailure> expectedError)
        {
            // Arrange
            _productTestTableManager.Cleanup();

            // Act
            var result = await _sut.Create(productDTO);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var actual = Assert.IsAssignableFrom<IList<ValidationFailure>>(badRequest.Value);

            var actualAnonymous = actual.Select(e => new { e.PropertyName, e.ErrorMessage });
            var expectedAnonymous = expectedError.Select(e => new { e.PropertyName, e.ErrorMessage });

            actualAnonymous.Should().BeEquivalentTo(expectedAnonymous);
        }

        [Fact]
        public async Task Update_ShouldUpdateProduct()
        {
            // Arrange
            _productTestTableManager.Cleanup();
            await _productTestTableManager.InsertProductAsync();

            var productDTO = new ProductDTO()
            {
                Name = "UpdatedProductName",
                Description = "UpdatedProductDescription",
                Price = 20.5M,
                AmountInStock = 15
            };

            var product = (await _productRepository.GetAllAsync()).First();

            // Act
            var result = await _sut.Update(product.Code, productDTO);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<Product>(ok.Value!);

            Assert.Equal("UpdatedProductName", actual.Name);
            Assert.Equal("UpdatedProductDescription", actual.Description);
            Assert.Equal(20.5M, actual.Price);
            Assert.Equal(15, actual.AmountInStock);

            var updatedProduct = await _productRepository.GetByIdAsync(product.ID);
            Assert.Equal("UpdatedProductName", updatedProduct.Name);
            Assert.Equal("UpdatedProductDescription", updatedProduct.Description);
            Assert.Equal(20.5M, updatedProduct.Price);
            Assert.Equal(15, updatedProduct.AmountInStock);
        }

        [Theory]
        [MemberData(nameof(GetInvalidProductDTOs))]
        public async Task Update_ShouldReturnBadRequestWhenInvalidProductDTO(ProductDTO productDTO, IList<ValidationFailure> expectedError)
        {
            // Arrange
            _productTestTableManager.Cleanup();

            // Act
            var result = await _sut.Update(1, productDTO);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var actual = Assert.IsAssignableFrom<IList<ValidationFailure>>(badRequest.Value);

            var actualAnonymous = actual.Select(e => new { e.PropertyName, e.ErrorMessage });
            var expectedAnonymous = expectedError.Select(e => new { e.PropertyName, e.ErrorMessage });

            actualAnonymous.Should().BeEquivalentTo(expectedAnonymous);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Update_ShouldReturnBadRequest_WhenInvalidProductCode(int invalidCode)
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
            var result = await _sut.Update(invalidCode, productDTO);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.INCORRECTFORMAT, badRequest.Value);
        }

        [Fact]
        public async Task Delete_ShouldDeleteProduct()
        {
            // Arrange
            _productTestTableManager.Cleanup();
            await _productTestTableManager.InsertProductAsync();
            var product = (await _productRepository.GetAllAsync()).First();

            // Act
            var result = await _sut.Delete(product.Code);

            // Assert
            Assert.Null(await _productRepository.GetByIdAsync(product.ID));
            var ok = Assert.IsType<OkObjectResult>(result);
            var anon = ok.Value!;

            var actual = new
            {
                ID = (Guid)anon.GetType().GetProperty("ID")!.GetValue(anon)!,
                Name = (string)anon.GetType().GetProperty("Name")!.GetValue(anon)!
            };

            Assert.Equal(product.ID, actual.ID);
            Assert.Equal("Name0", actual.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Delete_ShouldReturnBadRequest_WhenInvalidId(int productCode)
        {
            // Arrange
            _productTestTableManager.Cleanup();

            // Act
            var result = await _sut.Delete(productCode);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.INCORRECTFORMAT, badRequest.Value);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product("Name0", "Description0", 10, 10) { Code = 1 },

                new Product("Name1", "Description1", 10, 10) { Code = 2 },

                new Product("Name2", "Description2", 10, 10) { Code = 3 }
            };

            _productTestTableManager.Cleanup();
            await _productTestTableManager.InsertProductAsync(3);

            // Act
            var result = await _sut.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);

            products.Should().BeEquivalentTo(expectedProducts, options => options
                .Excluding(p => p.ID)
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(5)))
                .WhenTypeIs<DateTime>());
        }

        [Fact]
        public async Task GetAll_ShouldReturnBadRequest_WhenNoProducts()
        {
            // Arrange
            _productTestTableManager.Cleanup();

            // Act
            var result = await _sut.GetAll();

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
