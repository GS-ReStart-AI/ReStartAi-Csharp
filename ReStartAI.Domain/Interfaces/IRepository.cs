using System.Linq.Expressions;

namespace ReStartAI.Domain.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize);
        Task<T?> GetByIdAsync(string id);
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
    }
}