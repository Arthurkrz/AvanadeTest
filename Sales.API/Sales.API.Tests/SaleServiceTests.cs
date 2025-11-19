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
        public async Task Sell_ShouldInvokeRepositoryAddMethodAndMessagePublishMethod()
        {
            // Arrange
            _stockClientMock.Setup(s => s.ProductExistsAsync(
                It.IsAny<int>())).ReturnsAsync(true);

            _identityClientMock.Setup(i => i.BuyerExistsAsync(
                It.IsAny<int>())).ReturnsAsync(true);

            _saleRepositoryMock.Setup(r => r.GetById(
                It.IsAny<Guid>())).Returns(It.IsAny<Sale>());

            var sale = new Sale(1, 1, 1, SaleStatus.Pending);

            // Act
            await _sut.Sell(sale);

            // Assert
            _producerServiceMock.Verify(p => p.PublishProductSale(
                sale.SaleCode, sale.ProductCode, sale.SellAmount), Times.Once);

            _saleRepositoryMock.Verify(r => r.Add(sale), Times.Once);
        }

        [Fact]
        public async Task Sell_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            _stockClientMock.Setup(s => s.ProductExistsAsync(
                It.IsAny<int>())).ReturnsAsync(false);

            // Act & Assert
            var saex = await Assert.ThrowsAsync<SaleApiException>(async () => 
                await _sut.Sell(new Sale(1, 1, 1, SaleStatus.Pending)));

            Assert.Equal(ErrorMessages.INVALIDSALEREQUEST, saex.Message);
        }

        [Fact]
        public async Task Sell_ShouldThrowException_WhenBuyerDoesNotExist()
        {
            // Arrange
            _stockClientMock.Setup(s => s.ProductExistsAsync(
                It.IsAny<int>())).ReturnsAsync(true);

            _identityClientMock.Setup(i => i.BuyerExistsAsync(
                It.IsAny<int>())).ReturnsAsync(false);

            // Act & Assert
            var saex = await Assert.ThrowsAsync<SaleApiException>(async () =>
                await _sut.Sell(new Sale(1, 1, 1, SaleStatus.Pending)));

            Assert.Equal(ErrorMessages.INVALIDSALEREQUEST, saex.Message);
        }

        [Fact]
        public void UpdateSaleStatus_ShouldInvokeRepositoryUpdateStatusWithCompleted_WhenSuccessIsTrueAndNoErrorsExist()
        {
            // Arrange
            _saleRepositoryMock.Setup(r => r.IsSaleExistingByCode(
                It.IsAny<int>())).Returns(true);

            _saleRepositoryMock.Setup(r => r.GetByCode(
                It.IsAny<int>())).Returns(It.IsAny<Sale>());

            // Act
            _sut.UpdateSaleStatus(1, true, []);

            // Assert
            _saleRepositoryMock.Verify(r => r.UpdateStatus(
                1, SaleStatus.Completed), Times.Once);

            _saleRepositoryMock.Verify(r => r.UpdateStatus(
                1, SaleStatus.Rejected), Times.Never);
        }

        [Fact]
        public void UpdateSaleStatus_ShouldInvokeRepositoryUpdateStatusMethodWithRejectedAndThrowException_WhenSuccessIsFalseAndErrorsExist()
        {
            // Arrange
            _saleRepositoryMock.Setup(r => r.IsSaleExistingByCode(
                It.IsAny<int>())).Returns(true);

            _saleRepositoryMock.Setup(r => r.GetByCode(
                It.IsAny<int>())).Returns(It.IsAny<Sale>());

            // Act & Assert
            var saex = Assert.Throws<SaleApiException>(() => 
                _sut.UpdateSaleStatus(1, false, ["ERROR"]));

            _saleRepositoryMock.Verify(r => r.UpdateStatus(
                1, SaleStatus.Rejected), Times.Once);

            _saleRepositoryMock.Verify(r => r.UpdateStatus(
                1, SaleStatus.Completed), Times.Never);
        }

        [Fact]
        public void UpdateSaleStatus_ShouldThrowException_WhenSuccessIsTrueAndErrorsExist()
        {
            // Act & Assert
            var saex = Assert.Throws<SaleApiException>(() =>
                _sut.UpdateSaleStatus(1, true, ["ERROR"]));

            Assert.Equal(ErrorMessages.INVALIDSALESTATUSRESPONSE, saex.Message);
        }

        [Fact]
        public void UpdateSaleStatus_ShouldThrowException_WhenSaleDoesNotExist()
        {
            // Arrange
            _saleRepositoryMock.Setup(r => r.IsSaleExistingByCode(
                It.IsAny<int>())).Returns(false);

            // Act & Assert
            var saex = Assert.Throws<SaleApiException>(() =>
                _sut.UpdateSaleStatus(1, false, ["ERROR"]));

            Assert.Equal(ErrorMessages.SALENOTFOUND, saex.Message);
        }
    }
}