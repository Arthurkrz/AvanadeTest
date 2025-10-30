namespace Identity.API.Core.Common
{
    public class RegisterRequest
    {
        public RegisterRequest(string username, string name, string cpf, string password)
        {
            Username = username;
            Name = name;
            CPF = cpf;
            Password = password;
        }

        public string Username { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string CPF { get; set; } = default!;

        public string Password { get; set; } = default!;
    }
}
