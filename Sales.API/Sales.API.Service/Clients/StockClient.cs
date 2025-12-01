using Microsoft.Extensions.Configuration;
using Sales.API.Core.Contracts.Client;

namespace Sales.API.Service.Clients
{
    public class StockClient : IStockClient
    {
        private readonly HttpClient _httpClient;

        public StockClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config["Services:StockAPI"]!);
        }

        public async Task<bool> ProductExistsAsync(int productCode)
        {
            var response = await _httpClient.GetAsync($"/api/products/exists/{productCode}");
            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            return bool.TryParse(content, out var exists) && exists;
        }
    }
}
