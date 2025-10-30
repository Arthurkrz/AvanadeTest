using Identity.API.Core.Common;
using Identity.API.Core.Entities;

namespace Identity.API.Core.Contracts.Service
{
    public interface IAdminService
    {
        Admin Register(RegisterRequest request);

        bool Login(string username, string password);
    }
}
