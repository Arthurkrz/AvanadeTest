using Identity.API.Architecture;
using Identity.API.Core.Contracts.Repository;
using Identity.API.Core.Entities;
using Konscious.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Identity.API.Tests.Integration.Utilities
{
    public class BuyerTestTableManager
    {
        private readonly Context _context;
        private readonly IBaseRepository<Buyer> _buyerRepository;

        public BuyerTestTableManager(Context context, IBaseRepository<Buyer> buyerRepository)
        {
            _context = context;
            _buyerRepository = buyerRepository;
        }

        public void Cleanup() =>
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Buyers");

        public void InsertBuyer(int numberOfBuyersToInsert = 1)
        {
            for (int buyerNumber = 0; buyerNumber < numberOfBuyersToInsert; buyerNumber++)
            {
                var (passwordHash, salt) = HashPassword($"Password{buyerNumber}");

                var buyer = new Buyer($"Username{buyerNumber}", $"Name{buyerNumber}", $"CPF{buyerNumber}", 
                                      $"Email{buyerNumber}", $"PhoneNumber{buyerNumber}", 
                                      $"DeliveryAddress{buyerNumber}", passwordHash, salt,
                                      "Argon2id", "m=65536;i=3;p=1;len=32");

                _buyerRepository.Create(buyer);
            }
        }

        private (byte[] passwordHash, byte[] salt) HashPassword(string password)
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
