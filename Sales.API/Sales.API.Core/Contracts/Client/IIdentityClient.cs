namespace Sales.API.Core.Contracts.Client
{
    public interface IIdentityClient
    {
        Task<bool> BuyerExistsAsync(int buyerCPF);
    }
}
