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

        public Sale Add(Sale sale)
        {
            _context.Add(sale);
            _context.SaveChanges();

            return sale;
        }

        public Sale UpdateStatus(int saleCode, SaleStatus status)
        {
            var existingEntity = _context.Sales.FirstOrDefault(s => s.SaleCode == saleCode);

            existingEntity!.Status = status;
            _context.SaveChanges();

            return existingEntity;
        }

        public IEnumerable<Sale> GetAll() =>
            _context.Set<Sale>().AsQueryable();

        public Sale GetById(Guid id) => _context.Sales.Find(id)!;

        public Sale GetByCode(int saleCode) =>
            _context.Sales.FirstOrDefault(s => s.SaleCode == saleCode)!;

        public IEnumerable<Sale> GetByBuyer(int buyerCPF) =>
            _context.Sales.Where(s => s.BuyerCPF == buyerCPF).OrderBy(s => s.BuyerCPF);

        public IEnumerable<Sale> GetByProductCode(int productCode) =>
            _context.Sales.Where(s => s.ProductCode == productCode).OrderBy(s => s.ProductCode);

        public bool IsSaleExistingByCode(int saleCode) =>
            _context.Sales.Any(s => s.SaleCode == saleCode);
    }
}
