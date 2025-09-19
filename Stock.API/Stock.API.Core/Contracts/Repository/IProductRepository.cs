using Stock.API.Core.Common;
using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Repository
{
    public interface IProductRepository
    {
        Response<Product> Create(Product product);

        Response<Product> UpdateStock(Guid productId, int newAmountInStock);

        Response<Product> UpdateProduct(Guid productId, Product product);

        Response<Product> Delete(Guid id);

        IEnumerable<Product> GetAll();

        Response<Product> GetById(Guid productId);
    }
}
