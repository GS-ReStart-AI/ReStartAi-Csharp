using MongoDB.Driver;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Data;

namespace ReStartAI.Infrastructure.Repositories;

public class CandidaturaRepository : MongoRepository<Candidatura>, ICandidaturaRepository
{
    private readonly IMongoCollection<Candidatura> _collection;

    public CandidaturaRepository(MongoContext context) : base(context.Candidaturas)
    {
        _collection = context.Candidaturas;
    }

    public async Task<List<Candidatura>> GetByUserAsync(string userId)
    {
        return await _collection.Find(x => x.UserId == userId).ToListAsync();
    }

    public async Task<Candidatura?> GetByUserAndVagaAsync(string userId, string vagaId)
    {
        return await _collection.Find(x => x.UserId == userId && x.VagaId == vagaId).FirstOrDefaultAsync();
    }
}