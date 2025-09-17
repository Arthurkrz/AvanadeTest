using Stock.API.Core.Common;
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

        public Response<Product> Create(Product product)
        {
            try { _context.Products.Add(product); }
            catch (Exception ex) { return Response<Product>.Ko(0, new List<string> { ex.Message }); }

            _context.SaveChanges();
            return Response<Product>.Ok(product);
        }

        public IEnumerable<Product> GetAll() =>
            _context.Set<Product>().AsQueryable();

        public Response<Product> GetById(Guid productId)
        {
            try 
            { 
                var entity = _context.Products.Find(productId); 

                if (entity == null)
                    return Response<Product>.Ko(0, new List<string> { "Product not found." });

                return Response<Product>.Ok(entity);
            }

            catch (Exception ex) { return Response<Product>.Ko(0, new List<string> { ex.Message }); }
        }

        public Response<Product> UpdateStock(Guid productId, int newAmountInStock)
        {
            var existingEntity = _context.Products.Find(productId);
            if (existingEntity == null)
                return Response<Product>.Ko(0, new List<string> { "Product not found." });

            try { _context.Entry(existingEntity).CurrentValues
                          .SetValues(new { AmountInStock = newAmountInStock }); }

            catch (Exception ex) { return Response<Product>.Ko(0, new List<string> { ex.Message }); }

            _context.SaveChanges();
            return Response<Product>.Ok(existingEntity);
        }
    }
}
