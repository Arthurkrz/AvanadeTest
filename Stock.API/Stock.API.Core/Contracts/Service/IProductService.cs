using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Service
{
    public interface IProductService
    {
        Product Create(Product product);

        Product UpdateStock(int productCode, int sellAmount);

        Product UpdateProduct(int productCode, Product product);

        Product DeleteProduct(int productCode);

        Product GetByCode(int productCode);

        IEnumerable<Product> GetAll();
    }
}
