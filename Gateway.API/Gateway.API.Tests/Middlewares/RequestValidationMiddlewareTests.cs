using Gateway.API.Web.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gateway.API.Tests.Middlewares
{
    public class RequestValidationMiddlewareTests
    {
        private readonly RequestValidationMiddleware _sut;
        private readonly Mock<ILogger<RequestValidationMiddleware>> _loggerMock = new();

        public RequestValidationMiddlewareTests()
        {
            _sut = new RequestValidationMiddleware(
                next: (innerHttpContext) => Task.CompletedTask,
                _loggerMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn400_WhenContentTypeMissingOnPost()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Response.Body = new MemoryStream();

            // Act
            await _sut.InvokeAsync(context);

            // Assert
            Assert.Equal(400, context.Response.StatusCode);
            context.Response.Body.Position = 0;
            var responseText = new StreamReader(context.Response.Body).ReadToEnd();
            Assert.Contains("Content-Type header required.", responseText);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn400_WhenContentTypeMissingOnPut()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "PUT";
            context.Response.Body = new MemoryStream();

            // Act
            await _sut.InvokeAsync(context);

            // Assert
            Assert.Equal(400, context.Response.StatusCode);
            context.Response.Body.Position = 0;
            var responseText = new StreamReader(context.Response.Body).ReadToEnd();
            Assert.Contains("Content-Type header required.", responseText);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn200_WhenContentTypePresentOnPost()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/json";
            context.Response.Body = new MemoryStream();

            // Act
            await _sut.InvokeAsync(context);

            // Assert
            Assert.Equal(200, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn200_WhenContentTypePresentOnPut()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "PUT";
            context.Request.ContentType = "application/json";
            context.Response.Body = new MemoryStream();

            // Act
            await _sut.InvokeAsync(context);

            // Assert
            Assert.Equal(200, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn200_WhenMethodIsGet()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Response.Body = new MemoryStream();

            // Act
            await _sut.InvokeAsync(context);

            // Assert
            Assert.Equal(200, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn200_WhenMethodIsDelete()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Response.Body = new MemoryStream();

            // Act
            await _sut.InvokeAsync(context);

            // Assert
            Assert.Equal(200, context.Response.StatusCode);
        }
    }
}