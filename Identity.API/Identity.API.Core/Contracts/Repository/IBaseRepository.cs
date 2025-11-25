namespace Identity.API.Core.Contracts.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);

        Task<T> GetByUsernameAsync(string username);

        Task<T> UpdateAsync(T entity);
    }
}
