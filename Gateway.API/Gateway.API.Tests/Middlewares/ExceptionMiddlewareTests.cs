using Gateway.API.Web.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gateway.API.Tests.Middlewares
{
    public class ExceptionMiddlewareTests
    {
        private readonly ExceptionMiddleware _sut;
        private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock = new();

        public ExceptionMiddlewareTests()
        {
            _sut = new ExceptionMiddleware(
                next: (innerHttpContext) => throw new Exception("ERROR"), 
                _loggerMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn500_WhenExceptionThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await _sut.InvokeAsync(context);

            // Assert
            Assert.Equal(500, context.Response.StatusCode);

            context.Response.Body.Position = 0;
            var responseText = new StreamReader(context.Response.Body).ReadToEnd();

            Assert.Contains("Gateway error ocurred", responseText);

            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.AtLeastOnce
            );
        }
    }
}
