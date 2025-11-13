using ReStartAI.Domain.Entities;

namespace ReStartAI.Domain.Interfaces
{
    public interface ICandidaturaRepository
    {
        Task<IEnumerable<Candidatura>> GetAllAsync(int page, int pageSize);
        Task<Candidatura?> GetByIdAsync(string id);
        Task<Candidatura> CreateAsync(Candidatura entity);
        Task UpdateAsync(string id, Candidatura entity);
        Task DeleteAsync(string id);
        Task<int> CountAsync();
    }
}