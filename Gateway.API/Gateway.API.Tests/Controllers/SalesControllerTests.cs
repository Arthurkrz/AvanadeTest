using Gateway.API.Web.Contracts;
using Gateway.API.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gateway.API.Tests.Controllers
{
    public class SalesControllerTests
    {
        private readonly Mock<IProxyService> _proxyServiceMock = new();
        private readonly SalesController _sut;

        public SalesControllerTests()
        {
            _sut = new SalesController(_proxyServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task ProcessSale_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/processSale"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.ProcessSale();

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/processSale"),
                Times.Once);
        }

        [Fact]
        public async Task GetAllSales_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/all"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.GetAllSales();

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/all"),
                Times.Once);
        }

        [Fact]
        public async Task GetSalesByBuyerCPF_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/buyer/10"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.GetSalesByBuyerCPF(10);

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/buyer/10"),
                Times.Once);
        }

        [Fact]
        public async Task GetSalesByProductCode_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/procut/10"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.GetSalesByProductCode(10);

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/product/10"),
                Times.Once);
        }

        [Fact]
        public async Task GetByCode_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/10"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.GetByCode(10);

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Sales", "/api/sale/10"),
                Times.Once);
        }

        [Fact]
        public void ProcessSale_ShouldHaveAuthorizeAttributeWithBuyerRole()
        {
            // Arrange
            var methodInfo = typeof(SalesController).GetMethod("ProcessSale");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Buyer", authorizeAttribute.Roles);
        }

        [Fact]
        public void GetAllSales_ShouldHaveAuthorizeAttributeWithAdminRole()
        {
            // Arrange
            var methodInfo = typeof(SalesController).GetMethod("GetAllSales");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Admin", authorizeAttribute.Roles);
        }

        [Fact]
        public void GetSalesByBuyerCPF_ShouldHaveAuthorizeAttributeWithAdminRole()
        {
            // Arrange
            var methodInfo = typeof(SalesController).GetMethod("GetSalesByBuyerCPF");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Admin", authorizeAttribute.Roles);
        }

        [Fact]
        public void GetSalesByProductCode_ShouldHaveAuthorizeAttributeWithAdminRole()
        {
            // Arrange
            var methodInfo = typeof(SalesController).GetMethod("GetSalesByProductCode");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Admin", authorizeAttribute.Roles);
        }

        [Fact]
        public void GetByCode_ShouldHaveAuthorizeAttributeWithAdminRole()
        {
            // Arrange
            var methodInfo = typeof(SalesController).GetMethod("GetByCode");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Admin", authorizeAttribute.Roles);
        }
    }
}
