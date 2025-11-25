using Sales.API.Core.Contracts.Client;

namespace Sales.API.Tests.Integration.Utilities
{
    public class FakeIdentityClient : IIdentityClient
    {
        public Task<bool> BuyerExistsAsync(int buyerCPF) =>
            Task.FromResult(true);
    }
}
