using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Entities;

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
            return sale;
        }

        public IEnumerable<Sale> GetAll() =>
            _context.Set<Sale>().AsQueryable();

        public IEnumerable<Sale> GetByBuyer(Guid buyerId) =>
            _context.Sales.Where(s => s.BuyerID == buyerId).OrderBy(s => s.BuyerID);

        public IEnumerable<Sale> GetByProductId(Guid productId) =>
            _context.Sales.Where(s => s.ProductID == productId).OrderBy(s => s.ProductID);

        public Sale GetById(int id) =>
            _context.Sales.Find(id)!;
    }
}
