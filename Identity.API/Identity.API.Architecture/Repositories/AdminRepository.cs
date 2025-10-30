namespace Identity.API.Architecture.Repositories
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

        public Admin GetByUsername(string username) =>
            _context.Admins.FirstOrDefault(a => a.Username == username)!;

        public Admin Update(Admin admin)
        {
            var existingAdmin = GetByUsername(admin.Username);

            _context.Entry(existingAdmin!).CurrentValues.SetValues(admin);
            _context.SaveChanges();

            return admin;
        }
    }
}
