using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Entities;

namespace Stock.API.Architecture.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly Context _context;

        public ProductRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Product Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public Product UpdateStock(int productCode, int newAmountInStock)
        {
            var existingEntity = _context.Products.FirstOrDefault(p => p.Code == productCode);

            _context.Entry(existingEntity!).CurrentValues
                            .SetValues(new { AmountInStock = newAmountInStock });

            _context.SaveChanges();
            existingEntity!.AmountInStock = newAmountInStock;
            return existingEntity;
        }

        public Product UpdateProduct(int productCode, Product product)
        {
            var existingEntity = _context.Products.FirstOrDefault(p => p.Code == productCode)!;

            existingEntity.Name = product.Name;
            existingEntity.Description = product.Description;
            existingEntity.Price = product.Price;
            existingEntity.AmountInStock = product.AmountInStock;
            _context.SaveChanges();

            return product;
        }

        public Product Delete(int productCode)
        {
            var product = GetByCode(productCode);

            _context.Remove(product!);
            _context.SaveChanges();

            return product!;
        }

        public Product GetById(Guid productId) =>
            _context.Products.Find(productId)!;

        public Product GetByCode(int productCode) =>
            _context.Products.FirstOrDefault(p => p.Code == productCode)!;

        public IEnumerable<Product> GetAll() =>
            _context.Set<Product>().AsQueryable();
    }
}
