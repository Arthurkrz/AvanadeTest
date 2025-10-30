using Konscious.Security.Cryptography;
using Stock.API.Core.Contracts.Service;
using Stock.API.Service;
using System.Security.Cryptography;
using System.Text;

namespace Stock.API.Tests.Services
{
    public class PasswordHasherTests
    {
        private readonly IPasswordHasher _sut;

        public PasswordHasherTests()
        {
            _sut = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_ShouldHashPassword()
        {
            // Act
            var result = _sut.HashPassword("TestPassword123!");

            // Assert
            Assert.NotEmpty(result.passwordHash);
            Assert.NotEmpty(result.salt);
            Assert.NotEmpty(result.hashParams);

            Assert.Equal(32, result.passwordHash.Length);
            Assert.Equal(16, result.salt.Length);
            Assert.Equal("m=65536;i=3;p=1;len=32", result.hashParams);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsValid()
        {
            // Arrange
            string hashParams = "m=65536;i=3;p=1;len=32";
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes("TestPassword123!"))
            {
                Salt = salt,
                DegreeOfParallelism = 1,
                Iterations = 3,
                MemorySize = 65536
            };

            byte[] hash = argon2.GetBytes(32);

            // Act & Assert
            Assert.True(_sut.VerifyPassword("TestPassword123!", hash, salt, hashParams));
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsInvalid()
        {
            string hashParams = "m=65536;i=3;p=1;len=32";
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes("TestPassword123!"))
            {
                Salt = salt,
                DegreeOfParallelism = 1,
                Iterations = 3,
                MemorySize = 65536
            };

            byte[] hash = argon2.GetBytes(32);

            // Act & Assert
            Assert.False(_sut.VerifyPassword("WrongPassword", hash, salt, hashParams));
        }
    }
}
