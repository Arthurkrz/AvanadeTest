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
        public void Create_ShouldInvokeRepositoryAddMethod()
        {
            // Arrange
            _productValidatorMock.Setup(v => v.Validate(It.IsAny<Product>()))
                .Returns(new ValidationResult());

            // Act
            _sut.Create(new Product("Name", "Description", 10, 10));

            // Assert
            _productRepositoryMock.Verify(r => r.Create(
                It.IsAny<Product>()), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetInvalidProducts))]
        public void Create_ShouldThrowExceptionWithErrors_WhenValidationFails(Product product, IList<string> expectedErrors)
        {
            // Arrange
            _sut = new ProductService(_productRepositoryMock.Object, _productValidator, _producerServiceMock.Object);

            var expectedError = string.Join(", ", expectedErrors);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.Create(product));

            var expectedMessage = ErrorMessages.INVALIDREQUEST
                .Replace("{error}", expectedError);

            Assert.Equal(expectedMessage, ex.Error);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public void UpdateStock_ShouldInvokeRepositoryUpdateStockAndMessagePublisherMethod()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 10);

            _productRepositoryMock.Setup(
                r => r.GetByCode(It.IsAny<int>()))
                .Returns(product);

            // Act
            _sut.UpdateStock(1, 1, 1);

            // Assert
            _productRepositoryMock.Verify(r => r.UpdateStock(
                It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            _producerServiceMock.Verify(p => p.PublishSaleProcessed(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IList<string>>()), Times.Once);
        }

        [Fact]
        public void UpdateStock_ShouldInvokePublishMethodWithErrors_WhenProductNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(
                r => r.GetByCode(It.IsAny<int>()))
                .Returns((Product)null!);

            var errorMessageList = new List<string> 
            { ErrorMessages.PRODUCTNOTFOUND };

            // Act
            _sut.UpdateStock(1, 1, 1);

            // Assert
            _productRepositoryMock.Verify(
                r => r.GetByCode(It.IsAny<int>()), 
                Times.Once);

            _producerServiceMock.Verify(
                p => p.PublishSaleProcessed(It.IsAny<int>(), 
                It.IsAny<int>(), errorMessageList), Times.Once);
        }

        [Fact]
        public void UpdateProduct_ShouldInvokeRepositoryUpdateProductMethod()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 10);

            _productValidatorMock.Setup(
                v => v.Validate(It.IsAny<Product>()))
                .Returns(new ValidationResult());

            _productRepositoryMock.Setup(
                r => r.GetByCode(It.IsAny<int>()))
                .Returns(product);

            // Act
            _sut.UpdateProduct(1, product);

            // Assert
            _productRepositoryMock.Verify(r => r.UpdateProduct(
                1, It.IsAny<Product>()), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetInvalidProducts))]
        public void UpdateProduct_ShouldThrowExceptionWithErrors_WhenValidationFails(Product product, IList<string> expectedErrors)
        {
            // Arrange
            _sut = new ProductService(_productRepositoryMock.Object, _productValidator, _producerServiceMock.Object);

            _productRepositoryMock.Setup(
                r => r.GetByCode(It.IsAny<int>()))
                .Returns(product);

            var expectedError = string.Join(", ", expectedErrors);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.UpdateProduct(1, product));

            var expectedMessage = ErrorMessages.INVALIDREQUEST
                .Replace("{error}", expectedError);

            Assert.Equal(expectedMessage, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public void UpdateProduct_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(
                r => r.GetByCode(It.IsAny<int>()))
                .Returns((Product)null!);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.UpdateProduct(1, new Product("Name", "Description", 10, 10)));
        }

        [Fact]
        public void DeleteProduct_ShouldInvokeRepositoryDeleteMethod()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 10);

            _productRepositoryMock.Setup(
                r => r.GetByCode(It.IsAny<int>()))
                .Returns(product);

            // Act
            _sut.DeleteProduct(1);

            // Assert
            _productRepositoryMock.Verify(r => r.Delete(
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void DeleteProduct_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(
                r => r.GetByCode(It.IsAny<int>()))
                .Returns((Product)null!);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.DeleteProduct(1));
        }

        [Fact]
        public void GetAll_ShouldInvokeRepositoryGetAllMethod()
        {
            // Act & Assert
            _sut.GetAll();
            _productRepositoryMock.Verify(r => r.GetAll(), Times.Once);
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