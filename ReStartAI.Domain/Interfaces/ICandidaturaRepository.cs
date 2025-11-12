using ReStartAI.Domain.Entities;

namespace ReStartAI.Domain.Interfaces
{
    public interface ICandidaturaRepository
    {
        Task<IEnumerable<Candidatura>> GetAllAsync(int pageNumber, int pageSize);
        Task<Candidatura?> GetByIdAsync(string id);
        Task<Candidatura> CreateAsync(Candidatura entity);
        Task UpdateAsync(string id, Candidatura entity);
        Task DeleteAsync(string id);
    }
}