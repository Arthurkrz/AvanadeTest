using Gateway.API.Tests.Integration.Utilities;
using System.Net;
using System.Text;

namespace Gateway.API.Tests.Integration
{
    public class MiddlewareIntegrationTests
    {
        private readonly TestApplicationFactory _factory = new();

        [Fact]
        public async Task PostWithoutContentType_ShouldReturn400()
        {
            // Arrange
            var client = _factory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "/stock/create")
            {
                Content = new StringContent("{}", Encoding.UTF8)
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutWithoutContentType_ShouldReturn400()
        {
            // Arrange
            var client = _factory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Put, "/stock/update")
            {
                Content = new StringContent("{}", Encoding.UTF8)
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostWithContentType_ShouldReturn200()
        {
            // Arrange
            var client = _factory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "/stock/create")
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PutWithContentType_ShouldReturn200()
        {
            // Arrange
            var client = _factory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Put, "/stock/update")
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExceptionFromProxy_ShouldReturn500()
        {
            // Arrange
            var client = _factory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "/stock/exception");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Gateway error ocurred", content);
        }

        [Fact]
        public async Task LoggingMiddleware_ShouldNotBlockRequest()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/stock/all");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
