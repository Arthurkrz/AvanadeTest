using Identity.API.Core.Contracts.Service;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Identity.API.Service
{
    public class PasswordHasher : IPasswordHasher
    {
        public (byte[] passwordHash, byte[] salt, string hashParams) HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            int memorySize = 65536;
            int iterations = 3;
            int parallelism = 1;
            int hashLength = 32;

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = parallelism,
                Iterations = iterations,
                MemorySize = memorySize
            };

            byte[] hash = argon2.GetBytes(hashLength);
            string hashParams = $"m={memorySize};i={iterations};p={parallelism};len={hashLength}";

            return (hash, salt, hashParams);
        }

        public bool VerifyPassword(string password, byte[] storedHash, byte[] salt, string hashParams)
        {
            var parts = hashParams.Split(';')
                                  .Select(p => p.Split('='))
                                  .ToDictionary(k => k[0], v => int.Parse(v[1]));

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = parts["p"],
                Iterations = parts["i"],
                MemorySize = parts["m"]
            };

            byte[] computedHash = argon2.GetBytes(parts["len"]);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
    }
}
