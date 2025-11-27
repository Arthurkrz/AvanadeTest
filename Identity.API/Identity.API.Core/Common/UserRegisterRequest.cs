namespace Identity.API.Core.Common
{
    public class UserRegisterRequest
    {
        public UserRegisterRequest(string username, string name, string cpf, string password, string email, string phoneNumber, string deliveryAddress) 
        {
            Username = username;
            Name = name;
            CPF = cpf;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            DeliveryAddress = deliveryAddress;
        }

        public string Username { get; set; }

        public string Name { get; set; }

        public string CPF { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string DeliveryAddress { get; set; }
    }
}
