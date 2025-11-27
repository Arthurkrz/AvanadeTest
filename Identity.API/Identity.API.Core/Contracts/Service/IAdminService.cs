using Identity.API.Core.Common;
using Identity.API.Core.Entities;

namespace Identity.API.Core.Contracts.Service
{
    public interface IAdminService
    {
        Task<Admin> RegisterAsync(AdminRegisterRequest request);

        Task<bool> LoginAsync(string username, string password);

        Task<Admin> GetByUsernameAsync(string username);
    }
}
