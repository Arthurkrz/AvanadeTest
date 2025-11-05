using Identity.API.Core.Contracts.Repository;
using Identity.API.Core.Entities;

namespace Identity.API.Architecture.Repositories
{
    public class BuyerRepository : IBaseRepository<Buyer>
    {
        private readonly Context _context;

        public BuyerRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Buyer Create(Buyer buyer)
        {
            _context.Buyers.Add(buyer);
            _context.SaveChanges();

            return buyer;
        }

        public Buyer GetByUsername(string username) =>
            _context.Buyers.FirstOrDefault(b => b.Username == username)!;

        public Buyer Update(Buyer buyer)
        {
            var existingBuyer = GetByUsername(buyer.Username);

            _context.Entry(existingBuyer!).CurrentValues.SetValues(buyer);
            _context.SaveChanges();

            return buyer;
        }
    }
}
