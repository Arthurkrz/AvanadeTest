using System.Security.Cryptography;
using System.Text;

namespace Identity.API.Tests.Integration.Utilities
{
    public class AdminTestTableManager
    {
        private readonly Context _context;
        private readonly IAdminRepository _adminRepository;

        public AdminTestTableManager(Context context, IAdminRepository adminRepository)
        {
            _context = context;
            _adminRepository = adminRepository;
        }

        public void Cleanup() =>
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Admins");

        public void InsertAdmin(int numberOfAdminsToInsert = 1)
        {
            for (int adminNumber = 0; adminNumber < numberOfAdminsToInsert; adminNumber++)
            {
                var (passwordHash, salt) = hashPassword($"Password{adminNumber}");

                var admin = new Admin($"Username{adminNumber}", $"Name{adminNumber}",
                                      $"CPF{adminNumber}", passwordHash, salt,
                                      "Argon2id", "m=65536;i=3;p=1;len=32");

                _adminRepository.Create(admin);
            }
        }

        private (byte[] passwordHash, byte[] salt) hashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 1,
                Iterations = 3,
                MemorySize = 65536
            };

            return (argon2.GetBytes(32), salt);
        }
    }
}
