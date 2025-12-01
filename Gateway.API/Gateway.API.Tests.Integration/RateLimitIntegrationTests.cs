using Gateway.API.Tests.Integration.Utilities;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;

namespace Gateway.API.Tests.Integration
{
    public class RateLimitIntegrationTests
    {
        private readonly TestApplicationFactory _factory;
        private readonly HttpClient _client;

        public RateLimitIntegrationTests()
        {
            _factory = new TestApplicationFactory();
            _client = _factory.CreateClient();
        }


        [Fact]
        public async Task RateLimiter_ShouldReturn429()
        {
            // Arrange
            var factory = new TestApplicationFactory();
            var client = factory.CreateClient();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build()
                .GetSection("Jwt");

            var token = JwtHelper.CreateTestJwt("Admin", 
                config["Issuer"]!, config["Audience"]!, 
                config["Key"]!);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            HttpResponseMessage? lastResponse = null;
            for (int i = 0; i < 15; i++)
            {
                lastResponse = await client.GetAsync("/stock/all");
            }

            // Assert
            Assert.NotNull(lastResponse);
            Assert.Equal(HttpStatusCode.TooManyRequests, lastResponse.StatusCode);
            Assert.True(lastResponse!.StatusCode == HttpStatusCode.TooManyRequests, 
                "Expected 429 Too Many Requests but received: " + lastResponse.StatusCode);
        }
    }
}
