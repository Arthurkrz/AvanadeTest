using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Entities;

namespace Stock.API.Architecture.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly Context _context;

        public AdminRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Admin Create(Admin admin)
        {
            _context.Admins.Add(admin);
            _context.SaveChanges();

            return admin;
        }

        public Admin? GetByUsername(string username) => 
            _context.Admins.Find(username)!;

        public Admin Update(Admin admin)
        {
            var existingAdmin = _context.Admins.Find(admin.Username);

            _context.Entry(existingAdmin!).CurrentValues.SetValues(admin);
            _context.SaveChanges();

            return admin;
        }
    }
}
