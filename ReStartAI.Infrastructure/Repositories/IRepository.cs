using System.Linq.Expressions;

namespace ReStartAI.Infrastructure.Repositories;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(string id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> FindAsync(Expression<Func<T, bool>> filter);
    Task<T> InsertAsync(T entity);
    Task UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
}