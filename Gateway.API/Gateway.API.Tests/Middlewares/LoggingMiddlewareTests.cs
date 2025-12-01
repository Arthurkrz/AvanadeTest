using Gateway.API.Web.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gateway.API.Tests.Middlewares
{
    public class LoggingMiddlewareTests
    {
        private readonly LoggingMiddleware _sut;
        private readonly Mock<ILogger<LoggingMiddleware>> _loggerMock = new();

        public LoggingMiddlewareTests()
        {
            _sut = new LoggingMiddleware(
                next: (innerHttpContext) => Task.CompletedTask,
                _loggerMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_ShouldLogRequestAndResponse()
        {
            // Arrange
            bool nextCalled = false;

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await _sut.InvokeAsync(context);

            // Assert
            Assert.True(nextCalled);

            _loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeast(2));

            Assert.Equal(200, context.Response.StatusCode);
        }
    }
}
