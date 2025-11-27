namespace Identity.API.Core.Contracts.Service
{
    public interface IPasswordHasher
    {
        public (byte[] passwordHash, byte[] salt, string hashParams) HashPassword(string password);

        public bool VerifyPassword(string password, byte[] passwordHash, byte[] salt, string hashParams);
    }
}
