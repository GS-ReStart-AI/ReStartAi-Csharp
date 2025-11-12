using ReStartAI.Domain.Entities;

namespace ReStartAI.Domain.Interfaces
{
    public interface ICurriculoRepository
    {
        Task<IEnumerable<Curriculo>> GetAllAsync(int pageNumber, int pageSize);
        Task<Curriculo?> GetByIdAsync(string id);
        Task<Curriculo> CreateAsync(Curriculo entity);
        Task UpdateAsync(string id, Curriculo entity);
        Task DeleteAsync(string id);
    }
}