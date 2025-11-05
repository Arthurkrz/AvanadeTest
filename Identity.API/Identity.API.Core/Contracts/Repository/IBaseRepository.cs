namespace Identity.API.Core.Contracts.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        T Create(T entity);

        T GetByUsername(string username);

        T Update(T entity);
    }
}
