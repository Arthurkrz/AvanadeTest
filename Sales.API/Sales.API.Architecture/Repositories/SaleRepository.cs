using Microsoft.EntityFrameworkCore;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;

namespace Sales.API.Architecture.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly Context _context;

        public SaleRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(Context));
        }

        public async Task<Sale> AddAsync(Sale sale)
        {
            await _context.AddAsync(sale);
            await _context.SaveChangesAsync();

            return sale;
        }

        public async Task<Sale> UpdateStatusAsync(int saleCode, SaleStatus status)
        {
            var existingEntity = await 
                _context.Sales.FirstOrDefaultAsync(s => s.SaleCode == saleCode);

            existingEntity!.Status = status;
            await _context.SaveChangesAsync();

            return existingEntity;
        }

        public async Task<List<int>> GetPendingSalesAsync(TimeSpan maxAge)
        {
            var threshold = DateTime.UtcNow - maxAge;

            return await _context.Sales
                .Where(s => s.Status == SaleStatus.Pending && s.CreatedAt <= threshold)
                .Select(s => s.SaleCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetAllAsync() =>
            await _context.Set<Sale>().ToListAsync();

        public async Task<Sale> GetByIdAsync(Guid id) => 
            (await _context.Sales.FindAsync(id))!;

        public async Task<Sale> GetByCodeAsync(int saleCode) =>
            (await _context.Sales.FirstOrDefaultAsync(s => s.SaleCode == saleCode))!;

        public async Task<IEnumerable<Sale>> GetByBuyerAsync(int buyerCPF) =>
            await _context.Sales.Where(s => s.BuyerCPF == buyerCPF).OrderBy(s => s.BuyerCPF).ToListAsync();

        public async Task<IEnumerable<Sale>> GetByProductCodeAsync(int productCode) =>
            await _context.Sales.Where(s => s.ProductCode == productCode).OrderBy(s => s.ProductCode).ToListAsync();

        public async Task<bool> IsSaleExistingByCodeAsync(int saleCode) =>
            await _context.Sales.AnyAsync(s => s.SaleCode == saleCode);

    }
}
