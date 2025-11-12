using ReStartAI.Domain.Entities;

namespace ReStartAI.Infrastructure.Repositories;

public interface ICurriculoRepository : IRepository<Curriculo>
{
    Task<List<Curriculo>> GetByUserAsync(string userId);
    Task<Curriculo?> GetLastByUserAsync(string userId);
}