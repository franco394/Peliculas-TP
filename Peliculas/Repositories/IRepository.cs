using System.Linq.Expressions;

namespace Peliculas.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null);
        Task<T> GetOne(Expression<Func<T, bool>>? filter = null);
        Task<T> Create(T entity);
        Task<T> Update(T entity);
        Task DeleteOne(T entity);
    }
}