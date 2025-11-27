namespace Identity.API.Core.Entities
{
    public class Admin : Entity
    {
        private Admin() { }

        public Admin(string username, string name, string cpf, byte[] passwordHash, byte[] passwordSalt, string hashAlgorithm, string hashParams)
        {
            Username = username;
            Name = name;
            CPF = cpf;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            HashAlgorithm = hashAlgorithm;
            HashParams = hashParams;
        }

        public string Username { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string CPF { get; set; } = default!;

        public byte[] PasswordHash { get; set; } = default!;

        public byte[] PasswordSalt { get; set; } = default!;

        public string HashAlgorithm { get; set; } = default!;

        public string HashParams { get; set; } = default!;
    }
}
