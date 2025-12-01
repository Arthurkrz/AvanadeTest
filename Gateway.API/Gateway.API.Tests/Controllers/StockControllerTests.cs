using Gateway.API.Web.Contracts;
using Gateway.API.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gateway.API.Tests.Controllers
{
    public class StockControllerTests
    {
        private readonly Mock<IProxyService> _proxyServiceMock = new();
        private readonly StockController _sut;

        public StockControllerTests()
        {
            _sut = new StockController(_proxyServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task Create_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/create"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.Create();

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/create"), 
                Times.Once);
        }

        [Fact]
        public async Task Update_ShouldForwardToCorrectServiceAndPathAsync()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/update/10"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.Update(10);

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/update/10"),
                Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldForwardToCorrectServiceAndPathAsync()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/delete/10"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.Delete(10);

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/delete/10"),
                Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldForwardToCorrectServiceAndPathAsync()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/all"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.GetAll();

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/all"),
                Times.Once);
        }

        [Fact]
        public async Task GetByCode_ShouldForwardToCorrectServiceAndPathAsync()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/10"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.GetByCode(10);

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Stock", "/api/product/10"),
                Times.Once);
        }

        [Fact]
        public void Create_ShouldHaveAuthorizeAttributeWithAdminRole()
        {
            // Arrange
            var methodInfo = typeof(StockController).GetMethod("Create");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Admin", authorizeAttribute!.Roles);
        }

        [Fact]
        public void Update_ShouldHaveAuthorizeAttributeWithAdminRole()
        {
            // Arrange
            var methodInfo = typeof(StockController).GetMethod("Update");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Admin", authorizeAttribute!.Roles);
        }

        [Fact]
        public void Delete_ShouldHaveAuthorizeAttributeWithAdminRole()
        {
            // Arrange
            var methodInfo = typeof(StockController).GetMethod("Delete");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Admin", authorizeAttribute!.Roles);
        }

        [Fact]
        public void GetAll_ShouldHaveAuthorizeAttributeWithAdminAndBuyerRoles()
        {
            // Arrange
            var methodInfo = typeof(StockController).GetMethod("GetAll");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Admin,Buyer", authorizeAttribute!.Roles);
        }

        [Fact]
        public void GetByCode_ShouldHaveAuthorizeAttributeWithAdminAndBuyerRoles()
        {
            // Arrange
            var methodInfo = typeof(StockController).GetMethod("GetByCode");

            var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Admin,Buyer", authorizeAttribute!.Roles);
        }
    }
}
