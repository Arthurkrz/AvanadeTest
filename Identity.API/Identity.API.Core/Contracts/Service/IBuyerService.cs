using Identity.API.Core.Common;
using Identity.API.Core.Entities;

namespace Identity.API.Core.Contracts.Service
{
    public interface IBuyerService
    {
        Buyer Register(UserRegisterRequest request);

        bool Login(string username, string password);

        Buyer GetByUsername(string username);
    }
}
