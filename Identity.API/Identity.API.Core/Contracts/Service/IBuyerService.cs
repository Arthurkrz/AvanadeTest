using Identity.API.Core.Common;
using Identity.API.Core.Entities;

namespace Identity.API.Core.Contracts.Service
{
    public interface IBuyerService
    {
        Task<Buyer> RegisterAsync(UserRegisterRequest request);

        Task<bool> LoginAsync(string username, string password);

        Task<Buyer> GetByUsernameAsync(string username);

        Task<bool> IsExistingByCPFAsync(string cpf);
    }
}
