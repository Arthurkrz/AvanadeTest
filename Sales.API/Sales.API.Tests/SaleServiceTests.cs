using Moq;
using Sales.API.Core.Common;
using Sales.API.Core.Contracts.Client;
using Sales.API.Core.Contracts.RabbitMQ;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Contracts.Service;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;
using Sales.API.Service;

namespace Sales.API.Tests
{
    public class SaleServiceTests
    {
        private readonly Mock<IProducerService> _producerServiceMock = new();
        private readonly Mock<ISaleRepository> _saleRepositoryMock = new();

        private readonly Mock<IIdentityClient> _identityClientMock = new();
        private readonly Mock<IStockClient> _stockClientMock = new();

        private readonly ISaleService _sut;

        public SaleServiceTests()
        {
            _sut = new SaleService(_stockClientMock.Object, 
                                   _identityClientMock.Object, 
                                   _saleRepositoryMock.Object, 
                                   _producerServiceMock.Object);
        }

        [Fact]
        public async Task SellAsync_ShouldInvokeRepositoryAddMethodAndMessagePublishMethod()
        {
            // Arrange
            _stockClientMock.Setup(s => s.ProductExistsAsync(
                It.IsAny<int>())).ReturnsAsync(true);

            _identityClientMock.Setup(i => i.BuyerExistsAsync(
                It.IsAny<int>())).ReturnsAsync(true);

            _saleRepositoryMock.Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>())).ReturnsAsync(It.IsAny<Sale>());

            var sale = new Sale(1, 1, 1, SaleStatus.Pending);

            // Act
            await _sut.SellAsync(sale);

            // Assert
            _producerServiceMock.Verify(p => p.PublishProductSale(
                sale.SaleCode, sale.ProductCode, sale.SellAmount), Times.Once);

            _saleRepositoryMock.Verify(r => r.AddAsync(sale), Times.Once);
        }

        [Fact]
        public async Task SellAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            _stockClientMock.Setup(s => s.ProductExistsAsync(
                It.IsAny<int>())).ReturnsAsync(false);

            // Act & Assert
            var saex = await Assert.ThrowsAsync<SaleApiException>(() => 
                _sut.SellAsync(new Sale(1, 1, 1, SaleStatus.Pending)));

            Assert.Equal(ErrorMessages.INVALIDSALEREQUEST, saex.Message);
        }

        [Fact]
        public async Task SellAsync_ShouldThrowException_WhenBuyerDoesNotExist()
        {
            // Arrange
            _stockClientMock.Setup(s => s.ProductExistsAsync(
                It.IsAny<int>())).ReturnsAsync(true);

            _identityClientMock.Setup(i => i.BuyerExistsAsync(
                It.IsAny<int>())).ReturnsAsync(false);

            // Act & Assert
            var saex = await Assert.ThrowsAsync<SaleApiException>(() =>
                _sut.SellAsync(new Sale(1, 1, 1, SaleStatus.Pending)));

            Assert.Equal(ErrorMessages.INVALIDSALEREQUEST, saex.Message);
        }

        [Fact]
        public async Task UpdateSaleStatusAsync_ShouldInvokeRepositoryUpdateStatusWithCompleted_WhenSuccessIsTrueAndNoErrorsExist()
        {
            // Arrange
            _saleRepositoryMock.Setup(r => r.IsSaleExistingByCodeAsync(
                It.IsAny<int>())).ReturnsAsync(true);

            _saleRepositoryMock.Setup(r => r.GetByCodeAsync(
                It.IsAny<int>())).ReturnsAsync(It.IsAny<Sale>());

            // Act
            await _sut.UpdateSaleStatusAsync(1, true, []);

            // Assert
            _saleRepositoryMock.Verify(r => r.UpdateStatusAsync(
                1, SaleStatus.Completed), Times.Once);

            _saleRepositoryMock.Verify(r => r.UpdateStatusAsync(
                1, SaleStatus.Rejected), Times.Never);
        }

        [Fact]
        public async Task UpdateSaleStatusAsync_ShouldInvokeRepositoryUpdateStatusMethodWithRejectedAndThrowException_WhenSuccessIsFalseAndErrorsExist()
        {
            // Arrange
            _saleRepositoryMock.Setup(r => r.IsSaleExistingByCodeAsync(
                It.IsAny<int>())).ReturnsAsync(true);

            _saleRepositoryMock.Setup(r => r.GetByCodeAsync(It.IsAny<int>()))
                .ReturnsAsync(new Sale(1, 1, 1, SaleStatus.Pending));

            // Act & Assert
            var saex = await Assert.ThrowsAsync<SaleApiException>(() => 
                _sut.UpdateSaleStatusAsync(1, false, ["ERROR"]));

            _saleRepositoryMock.Verify(r => r.UpdateStatusAsync(
                1, SaleStatus.Rejected), Times.Once);

            _saleRepositoryMock.Verify(r => r.UpdateStatusAsync(
                1, SaleStatus.Completed), Times.Never);
        }

        [Fact]
        public async Task UpdateSaleStatusAsync_ShouldThrowException_WhenSuccessIsTrueAndErrorsExist()
        {
            // Act & Assert
            var saex = await Assert.ThrowsAsync<SaleApiException>(() =>
                _sut.UpdateSaleStatusAsync(1, true, ["ERROR"]));

            Assert.Equal(ErrorMessages.INVALIDSALESTATUSRESPONSE, saex.Message);
        }

        [Fact]
        public async Task UpdateSaleStatusAsync_ShouldThrowException_WhenSaleDoesNotExist()
        {
            // Arrange
            _saleRepositoryMock.Setup(r => r.IsSaleExistingByCodeAsync(
                It.IsAny<int>())).ReturnsAsync(false);

            // Act & Assert
            var saex = await Assert.ThrowsAsync<SaleApiException>(() =>
                _sut.UpdateSaleStatusAsync(1, false, ["ERROR"]));

            Assert.Equal(ErrorMessages.SALENOTFOUND, saex.Message);
        }
    }
}