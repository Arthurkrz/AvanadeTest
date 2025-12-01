using Gateway.API.Tests.Integration.Utilities;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Gateway.API.Tests.Integration.Gateways
{
    public class SalesGatewayIntegrationTests
    {
        private readonly TestApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly IConfigurationSection config;

        public SalesGatewayIntegrationTests()
        {
            _factory = new TestApplicationFactory();
            _client = _factory.CreateClient();

            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build()
                .GetSection("Jwt");
        }

        [Fact]
        public async Task ProcessSale_ShouldForwardCorrectly()
        {
            // Arrange
            var json = "{\"buyerCpf\":123,\"items\":[{\"productCode\":5,\"quantity\":1}]}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/sales/processSale", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(json, _factory.LastForwardedBody);
            Assert.EndsWith("/api/sale/processSale", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Post, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task GetAllSales_ShouldForwardCorrectly()
        {
            // Act
            var response = await _client.GetAsync("/sales/all");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.EndsWith("/api/sale/all", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Get, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task GetSalesByBuyerCPF_ShouldForwardCorrectly()
        {
            // Act
            var response = await _client.GetAsync("/sales/buyer/123456789");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.EndsWith("/api/sale/buyer/123456789", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Get, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task GetSalesByProductCode_ShouldForwardCorrectly()
        {
            // Act
            var response = await _client.GetAsync("/sales/product/10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.EndsWith("/api/sale/product/10", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Get, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task GetByCode_ShouldForwardCorrectly()
        {
            // Act
            var response = await _client.GetAsync("/sales/sale/10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.EndsWith("/api/sale/sale/10", _factory.LastForwardedUrl);
            Assert.Equal(HttpMethod.Get, _factory.LastForwardedMethod);
        }

        [Fact]
        public async Task ProcessSale_ShouldRejectAdminRole()
        {
            // Arrange
            var token = JwtHelper.CreateTestJwt("Admin",
                config["Issuer"]!, config["Audience"]!,
                config["Key"]!);

            var req = new HttpRequestMessage(HttpMethod.Post, "/sales/processSale");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(req);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAllSales_ShouldRejectBuyerRole()
        {
            // Arrange
            var token = JwtHelper.CreateTestJwt("Buyer",
                config["Issuer"]!, config["Audience"]!,
                config["Key"]!);

            var req = new HttpRequestMessage(HttpMethod.Get, "/sales/all");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(req);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetSalesByBuyerCPF_ShouldRejectBuyerRole()
        {
            // Arrange
            var token = JwtHelper.CreateTestJwt("Buyer",
                config["Issuer"]!, config["Audience"]!,
                config["Key"]!);

            var req = new HttpRequestMessage(HttpMethod.Get, "/sales/buyer/123456789");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(req);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetSalesByProductCode_ShouldRejectBuyerRole()
        {
            // Arrange
            var token = JwtHelper.CreateTestJwt("Buyer",
                config["Issuer"]!, config["Audience"]!,
                config["Key"]!);

            var req = new HttpRequestMessage(HttpMethod.Get, "/sales/product/10");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(req);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetByCode_ShouldRejectBuyerRole()
        {
            // Arrange
            var token = JwtHelper.CreateTestJwt("Buyer",
                config["Issuer"]!, config["Audience"]!,
                config["Key"]!);

            var req = new HttpRequestMessage(HttpMethod.Get, "/sales/sale/10");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(req);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ProcessSale_ShouldRejectBuyerMissingAuthentication()
        {
            // Act
            var response = await _client.PostAsync("/sales/create", 
                new StringContent("{}", Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
