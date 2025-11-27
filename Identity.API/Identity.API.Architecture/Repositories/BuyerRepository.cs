using Identity.API.Core.Contracts.Repository;
using Identity.API.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Architecture.Repositories
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly Context _context;

        public BuyerRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Buyer> CreateAsync(Buyer buyer)
        {
            await _context.Buyers.AddAsync(buyer);
            await _context.SaveChangesAsync();

            return buyer;
        }

        public async Task<Buyer> UpdateAsync(Buyer buyer)
        {
            var existingBuyer = await GetByUsernameAsync(buyer.Username);

            _context.Entry(existingBuyer!).CurrentValues.SetValues(buyer);
            await _context.SaveChangesAsync();

            return buyer;
        }

        public async Task<Buyer> GetByUsernameAsync(string username) =>
            (await _context.Buyers.FirstOrDefaultAsync(b => b.Username == username))!;

        public async Task<bool> IsExistingByCPFAsync(string cpf) =>
            await _context.Buyers.AnyAsync(b => b.CPF == cpf);
    }
}
