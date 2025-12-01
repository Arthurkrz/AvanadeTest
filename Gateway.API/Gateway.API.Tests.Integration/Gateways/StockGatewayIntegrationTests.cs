using Gateway.API.Tests.Integration.Utilities;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Gateway.API.Tests.Integration.Gateways
{
    public class StockGatewayIntegrationTests
    {
        private readonly TestApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly IConfigurationSection config;

        public StockGatewayIntegrationTests()
        {
            _factory = new TestApplicationFactory();
            _client = _factory.CreateClient();

            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build()
                .GetSection("Jwt");
        }

        [Fact]
        public async Task Create_ShouldForwardCorrectly()
        {
            // Arrange
            var json = "{\"name\":\"Test Product\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/stock/create", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.EndsWith("/stock/create", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Post, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task Update_ShouldForwardCorrectly()
        {
            // Arrange
            var json = "{\"name\":\"Test Product\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/stock/update/10", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.EndsWith("/stock/update/10", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Put, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task Delete_ShouldForwardCorrectly()
        {
            // Arrange
            var json = "{\"name\":\"Test Product\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/product/delete/10", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.EndsWith("/stock/delete/10", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Delete, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task GetAll_ShouldForwardCorrectly()
        {
            
            // Act
            var response = await _client.GetAsync("/stock/all");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.EndsWith("/stock/all", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Get, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task GetByCode_ShouldForwardCorrectly()
        {
            // Arrange
            var token = JwtHelper.CreateTestJwt("Buyer",
                config["Issuer"]!, config["Audience"]!,
                config["Key"]!);

            // Act
            var response = await _client.GetAsync("/stock/product/10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.EndsWith("/stock/product/10", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Get, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task Create_ShouldRejectBuyerRole()
        {
            // Arrange
            var token = JwtHelper.CreateTestJwt("Buyer",
                config["Issuer"]!, config["Audience"]!,
                config["Key"]!);

            var req = new HttpRequestMessage(HttpMethod.Post, "/stock/create");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(req);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldRejectBuyerRole()
        {
            // Arrange
            var token = JwtHelper.CreateTestJwt("Buyer",
                config["Issuer"]!, config["Audience"]!,
                config["Key"]!);

            var req = new HttpRequestMessage(HttpMethod.Post, "/stock/update/10");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(req);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldRejectBuyerRole()
        {
            // Arrange
            var token = JwtHelper.CreateTestJwt("Buyer",
                config["Issuer"]!, config["Audience"]!,
                config["Key"]!);

            var req = new HttpRequestMessage(HttpMethod.Post, "/stock/delete/10");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(req);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}