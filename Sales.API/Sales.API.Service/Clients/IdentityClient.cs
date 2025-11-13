using Microsoft.Extensions.Configuration;
using Sales.API.Core.Contracts.Client;

namespace Sales.API.Service.Clients
{
    public class IdentityClient : IIdentityClient
    {
        private readonly HttpClient _httpClient;

        public IdentityClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config["ServiceUrls:IdentityAPI"]!);
        }

        public async Task<bool> BuyerExistsAsync(Guid buyerId)
        {
            var response = await _httpClient.GetAsync($"/api/buyers/exists/{buyerId}");
            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            return bool.TryParse(content, out var exists) && exists;
        }
    }
}
