using ReStartAI.Domain.Entities;

namespace ReStartAI.Infrastructure.Repositories;

public interface ICandidaturaRepository : IRepository<Candidatura>
{
    Task<List<Candidatura>> GetByUserAsync(string userId);
    Task<Candidatura?> GetByUserAndVagaAsync(string userId, string vagaId);
}