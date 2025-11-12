using MongoDB.Driver;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Data;

namespace ReStartAI.Infrastructure.Repositories;

public class CurriculoRepository : MongoRepository<Curriculo>, ICurriculoRepository
{
    private readonly IMongoCollection<Curriculo> _collection;

    public CurriculoRepository(MongoContext context) : base(context.Curriculos)
    {
        _collection = context.Curriculos;
    }

    public async Task<List<Curriculo>> GetByUserAsync(string userId)
    {
        return await _collection.Find(x => x.UserId == userId).ToListAsync();
    }

    public async Task<Curriculo?> GetLastByUserAsync(string userId)
    {
        return await _collection
            .Find(x => x.UserId == userId)
            .SortByDescending(x => x.CriadoEm)
            .FirstOrDefaultAsync();
    }
}