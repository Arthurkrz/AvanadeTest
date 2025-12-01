using Gateway.API.Web.Contracts;
using Gateway.API.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gateway.API.Tests.Controllers
{
    public class IdentityControllerTests
    {
        private readonly Mock<IProxyService> _proxyServiceMock = new();
        private readonly IdentityController _sut;

        public IdentityControllerTests()
        {
            _sut = new IdentityController(_proxyServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task AdminRegister_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Identity", "/api/identity/admin/register"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.AdminRegister();

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Identity", "/api/identity/admin/register"),
                Times.Once);
        }

        [Fact]
        public async Task AdminLogin_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Identity", "/api/identity/admin/login"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.AdminLogin();

            // Assert
            Assert.IsType<OkResult>(result);
            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Identity", "/api/identity/admin/login"),
                Times.Once);
        }

        [Fact]
        public async Task BuyerRegister_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Identity", "/api/identity/buyer/register"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.BuyerRegister();

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Identity", "/api/identity/buyer/register"),
                Times.Once);
        }

        [Fact]
        public async Task BuyerLogin_ShouldForwardToCorrectServiceAndPath()
        {
            // Arrange
            _proxyServiceMock.Setup(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Identity", "/api/identity/buyer/login"))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _sut.BuyerLogin();

            // Assert
            Assert.IsType<OkResult>(result);

            _proxyServiceMock.Verify(p => p.ForwardAsync(
                It.IsAny<HttpContext>(), "Identity", "/api/identity/buyer/login"),
                Times.Once);
        }
    }
}
