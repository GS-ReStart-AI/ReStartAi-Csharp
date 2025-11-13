using ReStartAI.Domain.Entities;

namespace ReStartAI.Domain.Interfaces
{
    public interface IAppEventRepository
    {
        Task<IEnumerable<AppEvent>> GetAllAsync(int page, int pageSize);
        Task<AppEvent?> GetByIdAsync(string id);
        Task<AppEvent> CreateAsync(AppEvent entity);
        Task UpdateAsync(string id, AppEvent entity);
        Task DeleteAsync(string id);
        Task<int> CountAsync();
    }
}