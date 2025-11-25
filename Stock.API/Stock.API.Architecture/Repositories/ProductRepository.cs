using Microsoft.EntityFrameworkCore;
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

        public async Task<Product> CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> UpdateStockAsync(int productCode, int newAmountInStock)
        {
            var existingEntity = await 
                _context.Products.FirstOrDefaultAsync(p => p.Code == productCode);

            _context.Entry(existingEntity!).CurrentValues
                            .SetValues(new { AmountInStock = newAmountInStock });

            await _context.SaveChangesAsync();
            existingEntity!.AmountInStock = newAmountInStock;
            return existingEntity;
        }

        public async Task<Product> UpdateProductAsync(int productCode, Product product)
        {
            var existingEntity = await 
                _context.Products.FirstOrDefaultAsync(p => p.Code == productCode)!;

            existingEntity!.Name = product.Name;
            existingEntity.Description = product.Description;
            existingEntity.Price = product.Price;
            existingEntity.AmountInStock = product.AmountInStock;
            _context.SaveChanges();

            return product;
        }

        public async Task<Product> DeleteAsync(int productCode)
        {
            var product = await GetByCodeAsync(productCode);

            _context.Remove(product!);
            _context.SaveChanges();

            return product!;
        }

        public async Task<Product> GetByIdAsync(Guid productId) =>
            (await _context.Products.FindAsync(productId))!;

        public async Task<Product> GetByCodeAsync(int productCode) =>
            (await _context.Products.FirstOrDefaultAsync(p => p.Code == productCode))!;

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Set<Product>().ToListAsync();

        public async Task<bool> IsExistingByCodeAsync(int productCode) =>
            await _context.Products.AnyAsync(p => p.Code == productCode);
    }
}
