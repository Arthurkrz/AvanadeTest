using Sales.API.Core.Contracts.Client;

namespace Sales.API.Tests.Integration.Utilities
{
    public class FakeStockClient : IStockClient
    {
        public Task<bool> ProductExistsAsync(int productCode) =>
            Task.FromResult(true);
    }
}
