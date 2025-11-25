using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Service
{
    public interface IProductService
    {
        Task<Product> CreateAsync(Product product);

        Task UpdateStockAsync(int saleCode, int productCode, int sellAmount);

        Task<Product> UpdateProductAsync(int productCode, Product product);

        Task<Product> DeleteProductAsync(int productCode);

        Task<Product> GetByCodeAsync(int productCode);

        Task<bool> IsExistingByCodeAsync(int productCode);

        Task<IEnumerable<Product>> GetAllAsync();
    }
}
