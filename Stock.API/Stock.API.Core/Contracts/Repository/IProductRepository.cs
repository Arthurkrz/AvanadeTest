using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Repository
{
    public interface IProductRepository
    {
        Product Create(Product product);

        Product UpdateStock(Guid productId, int newAmountInStock);

        Product UpdateProduct(Guid productId, Product product);

        Product Delete(Guid id);

        IEnumerable<Product> GetAll();

        Product? GetById(Guid productId);
    }
}
