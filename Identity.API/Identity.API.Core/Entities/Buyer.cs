namespace Identity.API.Core.Entities
{
    public class Buyer : Entity
    {
        private Buyer() { }

        public Buyer(string name, string cpf, string email, string phoneNumber, string deliveryAddress, byte[] passwordHash, byte[] passwordSalt, string hashAlgorithm, string hashParams)
        {
            Name = name;
            CPF = cpf;
            Email = email;
            PhoneNumber = phoneNumber;
            DeliveryAddress = deliveryAddress;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            HashAlgorithm = hashAlgorithm;
            HashParams = hashParams;
        }

        public string Name { get; set; } = default!;

        public string CPF { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;

        public string DeliveryAddress { get; set; } = default!;

        public byte[] PasswordHash { get; set; } = default!;

        public byte[] PasswordSalt { get; set; } = default!;

        public string HashAlgorithm { get; set; } = default!;

        public string HashParams { get; set; } = default!;
    }
}
