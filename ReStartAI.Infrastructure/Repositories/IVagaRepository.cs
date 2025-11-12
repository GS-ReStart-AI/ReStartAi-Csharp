using ReStartAI.Domain.Entities;

namespace ReStartAI.Infrastructure.Repositories;

public interface IVagaRepository : IRepository<Vaga>
{
    Task<List<Vaga>> GetAtivasAsync();
    Task<List<Vaga>> SearchAsync(string? area, IEnumerable<string>? skillsMust, IEnumerable<string>? skillsNice);
}