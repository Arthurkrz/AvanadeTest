using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Repository
{
    public interface IProductRepository
    {
        Product Create(Product product);

        Product UpdateStock(int productCode, int newAmountInStock);

        Product UpdateProduct(int productCode, Product product);

        Product Delete(int productCode);

        Product GetById(Guid productId);

        Product GetByCode(int productCode);

        IEnumerable<Product> GetAll();
    }
}
