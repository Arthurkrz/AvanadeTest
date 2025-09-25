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
            try { _context.Admins.Add(admin); }
            catch (Exception ex) { throw new Exception("Could not create admin", ex); }

            _context.SaveChanges();
            return admin;
        }

        public Admin GetByUsername(string username)
        {
            try 
            { 
                var entity = _context.Admins.Find(username);

                if (entity is null) throw new Exception("Admin not found");

                return entity!;
            }

            catch (Exception ex) { throw new Exception("", ex); }
        }
    }
}
