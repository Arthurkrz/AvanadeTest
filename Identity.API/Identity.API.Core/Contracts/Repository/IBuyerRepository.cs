using Identity.API.Core.Entities;

namespace Identity.API.Core.Contracts.Repository
{
    public interface IBuyerRepository : IBaseRepository<Buyer>
    {
        Task<bool> IsExistingByCPFAsync(string cpf);
    }
}
