using Stock.API.Core.Common;
using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Service
{
    public interface IAdminService
    {
        Admin Register(RegisterRequest request);

        bool Login(string username, string password);
    }
}
