using FluentValidation;
using FluentValidation.Results;
using Moq;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.RabbitMQ;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Enum;
using Stock.API.Core.Validators;
using Stock.API.Service;

namespace Stock.API.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IProducerService> _producerServiceMock;
        private readonly Mock<IValidator<Product>> _productValidatorMock;
        private readonly IValidator<Product> _productValidator;
        private IProductService _sut;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _producerServiceMock = new Mock<IProducerService>();
            _productValidatorMock = new Mock<IValidator<Product>>();
            _productValidator = new ProductValidator();

            _sut = new ProductService(_productRepositoryMock.Object, 
                                      _productValidatorMock.Object, 
                                      _producerServiceMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldInvokeRepositoryAddMethod()
        {
            // Arrange
            _productValidatorMock.Setup(v => v.Validate(It.IsAny<Product>()))
                .Returns(new ValidationResult());

            // Act
            await _sut.CreateAsync(new Product("Name", "Description", 10, 10));

            // Assert
            _productRepositoryMock.Verify(r => r.CreateAsync(
                It.IsAny<Product>()), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetInvalidProducts))]
        public async Task CreateAsync_ShouldThrowExceptionWithErrors_WhenValidationFails(Product product, IList<string> expectedErrors)
        {
            // Arrange
            _sut = new ProductService(_productRepositoryMock.Object, _productValidator, _producerServiceMock.Object);

            var expectedError = string.Join(", ", expectedErrors);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<StockApiException>(() =>
                _sut.CreateAsync(product));

            var expectedMessage = ErrorMessages.INVALIDREQUEST
                .Replace("{error}", expectedError);

            Assert.Equal(expectedMessage, ex.Error);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public async Task UpdateStockAsync_ShouldInvokeRepositoryUpdateStockAndMessagePublisherMethod()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 10);

            _productRepositoryMock.Setup(
                r => r.GetByCodeAsync(It.IsAny<int>()))
                .ReturnsAsync(product);

            // Act
            await _sut.UpdateStockAsync(1, 1, 1);

            // Assert
            _productRepositoryMock.Verify(r => r.UpdateStockAsync(
                It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            _producerServiceMock.Verify(p => p.PublishSaleProcessedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IList<string>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStockAsync_ShouldInvokePublishMethodWithErrors_WhenProductNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(
                r => r.GetByCodeAsync(It.IsAny<int>()))
                .ReturnsAsync((Product)null!);

            var errorMessageList = new List<string> 
            { ErrorMessages.PRODUCTNOTFOUND };

            // Act
            await _sut.UpdateStockAsync(1, 1, 1);

            // Assert
            _productRepositoryMock.Verify(
                r => r.GetByCodeAsync(It.IsAny<int>()), 
                Times.Once);

            _producerServiceMock.Verify(
                p => p.PublishSaleProcessedAsync(It.IsAny<int>(), 
                It.IsAny<int>(), errorMessageList), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldInvokeRepositoryUpdateProductMethod()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 10);

            _productValidatorMock.Setup(
                v => v.Validate(It.IsAny<Product>()))
                .Returns(new ValidationResult());

            _productRepositoryMock.Setup(
                r => r.IsExistingByCodeAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            await _sut.UpdateProductAsync(1, product);

            // Assert
            _productRepositoryMock.Verify(r => r.UpdateProductAsync(
                1, It.IsAny<Product>()), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetInvalidProducts))]
        public async Task UpdateProductAsync_ShouldThrowExceptionWithErrors_WhenValidationFails(Product product, IList<string> expectedErrors)
        {
            // Arrange
            _sut = new ProductService(_productRepositoryMock.Object, _productValidator, _producerServiceMock.Object);

            _productRepositoryMock.Setup(
                r => r.IsExistingByCodeAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var expectedError = string.Join(", ", expectedErrors);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<StockApiException>(() =>
                _sut.UpdateProductAsync(1, product));

            var expectedMessage = ErrorMessages.INVALIDREQUEST
                .Replace("{error}", expectedError);

            Assert.Equal(expectedMessage, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(
                r => r.GetByCodeAsync(It.IsAny<int>()))
                .ReturnsAsync((Product)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<StockApiException>(() =>
                _sut.UpdateProductAsync(1, new Product("Name", "Description", 10, 10)));
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldInvokeRepositoryDeleteMethod()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 10);

            _productRepositoryMock.Setup(
                r => r.GetByCodeAsync(It.IsAny<int>()))
                .ReturnsAsync(product);

            // Act
            await _sut.DeleteProductAsync(1);

            // Assert
            _productRepositoryMock.Verify(r => r.DeleteAsync(
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(
                r => r.GetByCodeAsync(It.IsAny<int>()))
                .ReturnsAsync((Product)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<StockApiException>(() =>
                _sut.DeleteProductAsync(1));
        }

        [Fact]
        public async Task GetAllAsync_ShouldInvokeRepositoryGetAllMethod()
        {
            // Act & Assert
            await _sut.GetAllAsync();
            _productRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        public static IEnumerable<object[]> GetInvalidProducts()
        {
            yield return new object[]
            {
                new Product("", "Description", 10, 10),

                new List<string>
                {
                    "Product name must not be null or empty"
                }
            };

            yield return new object[]
            {
                new Product(new string('a', 101), "Description", 10, 10),

                new List<string>
                {
                    "Product name must not exceed 100 characters"
                }
            };

            yield return new object[]
            {
                new Product("Name", "", 10, 10),

                new List<string>
                {
                    "Product description must not be null or empty"
                }
            };

            yield return new object[]
            {
                new Product("Name", new string('a', 501), 10, 10),

                new List<string>
                {
                    "Product description must not exceed 500 characters"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 1000000, 10),

                new List<string>
                {
                    "Price cannot be equal or exceed 1.000.000"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 1000001, 10),

                new List<string>
                {
                    "Price cannot be equal or exceed 1.000.000"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 0, 10),

                new List<string>
                {
                    "Price cannot be zero or negative"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", -1, 10),

                new List<string>
                {
                    "Price cannot be zero or negative"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 10, 1000000),

                new List<string>
                {
                    "Amount in stock cannot be equal or exceed 1.000.000"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 10, 1000001),

                new List<string>
                {
                    "Amount in stock cannot be equal or exceed 1.000.000"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 10, -10),

                new List<string>
                {
                    "Amount in stock cannot be negative"
                }
            };

            yield return new object[]
            {
                new Product("", "", 0, -1),

                new List<string>
                {
                    "Product name must not be null or empty",
                    "Product description must not be null or empty",
                    "Price cannot be zero or negative",
                    "Amount in stock cannot be negative"
                }
            };

            yield return new object[]
            {
                new Product(new string('a', 101), new string('a', 501), 1000001, 1000001),

                new List<string>
                {
                    "Product name must not exceed 100 characters",
                    "Product description must not exceed 500 characters",
                    "Price cannot be equal or exceed 1.000.000",
                    "Amount in stock cannot be equal or exceed 1.000.000"
                }
            };
        }
    }
}