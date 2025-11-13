using ReStartAI.Domain.Entities;

namespace ReStartAI.Domain.Interfaces
{
    public interface IVagaRepository
    {
        Task<IEnumerable<Vaga>> GetAllAsync(int page, int pageSize);
        Task<Vaga?> GetByIdAsync(string id);
        Task<Vaga> CreateAsync(Vaga entity);
        Task UpdateAsync(string id, Vaga entity);
        Task DeleteAsync(string id);
        Task<int> CountAsync();
    }
}