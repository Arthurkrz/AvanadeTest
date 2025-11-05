namespace Identity.API.Core.Common
{
    public class AdminRegisterRequest
    {
        public AdminRegisterRequest(string username, string name, string cpf, string password)
        {
            Username = username;
            Name = name;
            CPF = cpf;
            Password = password;
        }

        public string Username { get; set; }

        public string Name { get; set; }

        public string CPF { get; set; }

        public string Password { get; set; }
    }
}
