using Gateway.API.Tests.Integration.Utilities;
using System.Net;
using System.Text;

namespace Gateway.API.Tests.Integration.Gateways
{
    public class IdentityGatewayIntegrationTests
    {
        private readonly TestApplicationFactory _factory;
        private readonly HttpClient _client;

        public IdentityGatewayIntegrationTests()
        {
            _factory = new TestApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task AdminRegister_ShouldForwardCorrectly()
        {
            // Arrange
            var json = "{\"email\":\"a@a.com\",\"password\":\"123\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/identity/admin/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.EndsWith("/identity/admin/register", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Post, _factory.LastForwardedMethod);
            Assert.Equal(json, _factory.LastForwardedBody);
        }

        [Fact]
        public async Task AdminLogin_ShouldForwardCorrectly()
        {
            // Arrange
            var json = "{\"email\":\"a@a.com\",\"password\":\"123\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/identity/admin/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.EndsWith("/identity/admin/login", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Post, _factory.LastForwardedMethod);
            Assert.Equal(json, _factory.LastForwardedBody);
        }

        [Fact]
        public async Task BuyerRegister_ShouldForwardCorrectly()
        {
            // Arrange
            var json = "{\"email\":\"a@a.com\",\"password\":\"123\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/identity/buyer/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.EndsWith("/identity/buyer/register", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Post, _factory.LastForwardedMethod);
            Assert.Equal(json, _factory.LastForwardedBody);
        }

        [Fact]
        public async Task BuyerLogin_ShouldForwardCorrectly()
        {
            // Arrange
            var json = "{\"email\":\"a@a.com\",\"password\":\"123\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/identity/buyer/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.EndsWith("/identity/buyer/login", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Post, _factory.LastForwardedMethod);
            Assert.Equal(json, _factory.LastForwardedBody);
        }
    }
}
