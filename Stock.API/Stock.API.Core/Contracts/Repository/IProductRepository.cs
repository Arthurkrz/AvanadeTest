using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Repository
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);

        Task<Product> UpdateStockAsync(int productCode, int newAmountInStock);

        Task<Product> UpdateProductAsync(int productCode, Product product);

        Task<Product> DeleteAsync(int productCode);

        Task<Product> GetByIdAsync(Guid productId);

        Task<Product> GetByCodeAsync(int productCode);

        Task<IEnumerable<Product>> GetAllAsync();

        Task<bool> IsExistingByCodeAsync(int productCode);
    }
}
