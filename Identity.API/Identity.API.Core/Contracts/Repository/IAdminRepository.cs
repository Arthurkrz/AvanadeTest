using Identity.API.Core.Entities;

namespace Identity.API.Core.Contracts.Repository
{
    public interface IAdminRepository
    {
        public Admin Create(Admin admin);

        public Admin GetByUsername(string username);

        public Admin Update(Admin admin);
    }
}
