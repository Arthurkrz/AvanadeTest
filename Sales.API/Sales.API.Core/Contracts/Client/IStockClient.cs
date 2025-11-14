namespace Sales.API.Core.Contracts.Client
{
    public interface IStockClient
    {
        Task<bool> ProductExistsAsync(int productCode);
    }
}
