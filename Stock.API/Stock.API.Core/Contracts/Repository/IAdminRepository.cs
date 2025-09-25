using Stock.API.Core.Entities;

namespace Stock.API.Core.Contracts.Repository
{
    public interface IAdminRepository
    {
        public Admin Create(Admin admin);

        public Admin GetByUsername(string username);
    }
}
