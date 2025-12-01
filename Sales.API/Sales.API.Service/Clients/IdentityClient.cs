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
            _httpClient.BaseAddress = new Uri(config["Services:IdentityAPI"]!);
        }

        public async Task<bool> BuyerExistsAsync(int buyerCPF)
        {
            var response = await _httpClient.GetAsync($"/api/buyers/exists/{buyerCPF}");
            if (!response.IsSuccessStatusCode) return false;
            
            var content = await response.Content.ReadAsStringAsync();
            return bool.TryParse(content, out var exists) && exists;
        }
    }
}
