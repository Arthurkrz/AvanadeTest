using Identity.API.Core.Contracts.Repository;
using Identity.API.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Architecture.Repositories
{
    public class AdminRepository : IBaseRepository<Admin>
    {
        private readonly Context _context;

        public AdminRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Admin> CreateAsync(Admin admin)
        {
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();

            return admin;
        }

        public async Task<Admin> UpdateAsync(Admin admin)
        {
            var existingAdmin = await GetByUsernameAsync(admin.Username);

            _context.Entry(existingAdmin!).CurrentValues.SetValues(admin);
            await _context.SaveChangesAsync();

            return admin;
        }

        public async Task<Admin> GetByUsernameAsync(string username) =>
            (await _context.Admins.FirstOrDefaultAsync(a => a.Username == username))!;
    }
}
