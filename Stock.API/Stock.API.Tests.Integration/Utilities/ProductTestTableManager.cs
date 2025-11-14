using Microsoft.EntityFrameworkCore;
using Stock.API.Architecture;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Entities;

namespace Stock.API.Tests.Integration.Utilities
{
    public class ProductTestTableManager
    {
        private readonly Context _context;
        private readonly IProductRepository _productRepository;

        public ProductTestTableManager(Context context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public void Cleanup() =>
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Products");

        public void InsertProduct(int numberOfProductsToInsert = 1)
        {
            for (int productNumber = 0; productNumber < numberOfProductsToInsert; productNumber++)
            {
                var product = new Product($"Name{productNumber}", 
                                          $"Description{productNumber}", 10, 10);

                product.Code = productNumber + 1;

                _productRepository.Create(product);
            }
        }
    }
}
