using Stock.API.Core.Common;
using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Service
{
    public interface IProductService
    {
        Response<Product> Create(Product product);

        Response<Product> UpdateStock(Guid productId, int newAmountInStock);

        Response<Product> UpdateProduct(Guid productId, Product product);

        Response<Product> DeleteProduct(Guid id);

        IEnumerable<Product> GetAll();
    }
}
