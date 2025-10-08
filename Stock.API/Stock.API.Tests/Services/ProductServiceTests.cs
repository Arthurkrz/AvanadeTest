using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Enum;
using Stock.API.Service;

namespace Stock.API.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IValidator<Product>> _productValidatorMock;
        private readonly IProductService _sut;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productValidatorMock = new Mock<IValidator<Product>>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_productRepositoryMock.Object);
            serviceCollection.AddSingleton(_productValidatorMock.Object);

            _sut = new ProductService(_productRepositoryMock.Object, _productValidatorMock.Object);
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

        [Fact]
        public void Create_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            _productValidatorMock.Setup(x => x.Validate(It.IsAny<Product>()))
                .Returns(new ValidationResult(new List<ValidationFailure> 
                { new ValidationFailure("Name", "Name is required") }));

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.Create(new Product("Name", "Description", 10, 10)));

            var expectedMessage = ErrorMessages.INVALIDREQUEST
                .Replace("{error}", "Name is required");

            Assert.Equal(expectedMessage, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public void UpdateStock_ShouldInvokeRepositoryUpdateStockMethod()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 10);

            _productRepositoryMock.Setup(
                r => r.GetById(It.IsAny<Guid>()))
                .Returns(product);

            // Act
            _sut.UpdateStock(new Guid(), 1);

            // Assert
            _productRepositoryMock.Verify(r => r.UpdateStock(
                It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void UpdateStock_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(
                r => r.GetById(It.IsAny<Guid>()))
                .Returns((Product)null!);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.UpdateStock(new Guid(), 1));
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
                r => r.GetById(It.IsAny<Guid>()))
                .Returns(product);

            // Act
            _sut.UpdateProduct(new Guid(), new Product("Name", "Description", 10, 10));

            // Assert
            _productRepositoryMock.Verify(r => r.UpdateProduct(
                It.IsAny<Guid>(), It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public void UpdateProduct_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 10);

            _productValidatorMock.Setup(x => x.Validate(It.IsAny<Product>()))
                .Returns(new ValidationResult(new List<ValidationFailure> 
                { new ValidationFailure("Name", "Name is required") }));

            _productRepositoryMock.Setup(
                r => r.GetById(It.IsAny<Guid>()))
                .Returns(product);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.UpdateProduct(new Guid(), new Product("Name", "Description", 10, 10)));

            var expectedMessage = ErrorMessages.INVALIDREQUEST
                .Replace("{error}", "Name is required");

            Assert.Equal(expectedMessage, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public void UpdateProduct_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(
                r => r.GetById(It.IsAny<Guid>()))
                .Returns((Product)null!);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.UpdateProduct(new Guid(), new Product("Name", "Description", 10, 10)));
        }

        [Fact]
        public void DeleteProduct_ShouldInvokeRepositoryDeleteMethod()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 10);

            _productRepositoryMock.Setup(
                r => r.GetById(It.IsAny<Guid>()))
                .Returns(product);

            // Act
            _sut.DeleteProduct(new Guid());

            // Assert
            _productRepositoryMock.Verify(r => r.Delete(
                It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void DeleteProduct_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(
                r => r.GetById(It.IsAny<Guid>()))
                .Returns((Product)null!);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.DeleteProduct(new Guid()));
        }

        [Fact]
        public void GetAll_ShouldInvokeRepositoryGetAllMethod()
        {
            // Act & Assert
            _sut.GetAll();
            _productRepositoryMock.Verify(r => r.GetAll(), Times.Once);
        }
    }
}