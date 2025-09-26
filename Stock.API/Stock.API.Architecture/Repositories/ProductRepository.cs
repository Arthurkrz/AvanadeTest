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
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public Product Delete(Guid id)
        {
            var product = GetById(id);

            if (product is null) 
                throw new StockApiException(ErrorMessages.PRODUCTNOTFOUND, 
                                            ErrorType.NotFound);
            _context.Remove(product);
            _context.SaveChanges();
            return product;
        }

        public IEnumerable<Product> GetAll() =>
            _context.Set<Product>().AsQueryable();

        public Product GetById(Guid productId)
        {
            var entity = _context.Products.Find(productId);

            if (entity is null)
                throw new StockApiException(ErrorMessages.PRODUCTNOTFOUND, 
                                            ErrorType.NotFound);
            return entity;
        }

        public Product UpdateProduct(Guid productId, Product product)
        {
            var existingEntity = _context.Set<Product>().Find(productId);

            if (existingEntity is null)
                throw new StockApiException(ErrorMessages.PRODUCTNOTFOUND, 
                                            ErrorType.NotFound);

            _context.Entry(existingEntity).CurrentValues.SetValues(product);
            _context.SaveChanges();
            return product;
        }

        public Product UpdateStock(Guid productId, int newAmountInStock)
        {
            var existingEntity = _context.Products.Find(productId);

            if (existingEntity == null)
            throw new StockApiException(ErrorMessages.PRODUCTNOTFOUND, 
                                        ErrorType.NotFound);

            _context.Entry(existingEntity).CurrentValues
                            .SetValues(new { AmountInStock = newAmountInStock });

            _context.SaveChanges();
            existingEntity.AmountInStock = newAmountInStock;
            return existingEntity;
        }
    }
}
