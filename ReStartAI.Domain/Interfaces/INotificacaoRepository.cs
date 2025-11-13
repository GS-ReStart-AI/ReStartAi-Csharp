using ReStartAI.Domain.Entities;

namespace ReStartAI.Domain.Interfaces
{
    public interface INotificacaoRepository
    {
        Task<IEnumerable<Notificacao>> GetAllAsync(int page, int pageSize);
        Task<Notificacao?> GetByIdAsync(string id);
        Task<Notificacao> CreateAsync(Notificacao entity);
        Task UpdateAsync(string id, Notificacao entity);
        Task DeleteAsync(string id);
        Task<int> CountAsync();
    }
}