using Stock.API.Core.Common;
using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Service
{
    public interface IProductService
    {
        Product Create(Product product);

        Product UpdateStock(Guid productId, int newAmountInStock);

        Product UpdateProduct(Guid productId, Product product);

        Product DeleteProduct(Guid id);

        IEnumerable<Product> GetAll();
    }
}
