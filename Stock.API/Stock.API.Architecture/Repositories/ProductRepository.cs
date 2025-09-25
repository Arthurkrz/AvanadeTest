using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Entities;
using Stock.API.Core.Enum;

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
            try { _context.Products.Add(product); }
            catch (Exception ex) { throw new StockApiException(new List<string> { ex.Message }, (ErrorType)4); }

            _context.SaveChanges();
            return product;
        }

        public Product Delete(Guid id)
        {
            var product = GetById(id);
            if (product is null) throw new StockApiException(new List<string> { "Product not found." }, (ErrorType)2);

            try { _context.Remove(product); }
            catch (Exception ex) { throw new StockApiException(new List<string> { ex.Message }, (ErrorType)4); }

            return product;
        }

        public IEnumerable<Product> GetAll() =>
            _context.Set<Product>().AsQueryable();

        public Product GetById(Guid productId)
        {
            try 
            { 
                var entity = _context.Products.Find(productId);

                if (entity is null)
                    throw new StockApiException(new List<string> { "Product not found" }, (ErrorType)2);

                return entity;
            }

            catch (Exception ex) { throw new StockApiException(new List<string> { ex.Message }, (ErrorType)4); }
        }

        public Product UpdateProduct(Guid productId, Product product)
        {
            var existingEntity = _context.Set<Product>().Find(productId);

            if (existingEntity is null)
                throw new StockApiException(new List<string> { "Product not found." }, (ErrorType)2);

            try { _context.Entry(existingEntity).CurrentValues.SetValues(product); }
            catch (Exception ex) { throw new StockApiException(new List<string> { ex.Message }, (ErrorType)4); }

            return product;
        }

        public Product UpdateStock(Guid productId, int newAmountInStock)
        {
            Product? existingEntity = new("Placeholder", "Placeholder", 1, 1);

            try
            {
                existingEntity = _context.Products.Find(productId);

                if (existingEntity == null)
                throw new StockApiException(new List<string> { "Product not found." }, (ErrorType)2);

                _context.Entry(existingEntity).CurrentValues
                              .SetValues(new { AmountInStock = newAmountInStock });
            }

            catch (Exception ex) { throw new StockApiException(new List<string> { ex.Message }, (ErrorType)4); }

            _context.SaveChanges();

            existingEntity.AmountInStock = newAmountInStock;
            return existingEntity;
        }
    }
}
