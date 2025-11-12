using MongoDB.Driver;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Data;

namespace ReStartAI.Infrastructure.Repositories;

public class VagaRepository : MongoRepository<Vaga>, IVagaRepository
{
    private readonly IMongoCollection<Vaga> _collection;

    public VagaRepository(MongoContext context) : base(context.Vagas)
    {
        _collection = context.Vagas;
    }

    public async Task<List<Vaga>> GetAtivasAsync()
    {
        return await _collection.Find(v => v.Ativo).ToListAsync();
    }

    public async Task<List<Vaga>> SearchAsync(string? area, IEnumerable<string>? skillsMust, IEnumerable<string>? skillsNice)
    {
        var fb = Builders<Vaga>.Filter;
        var filters = new List<FilterDefinition<Vaga>> { fb.Eq(v => v.Ativo, true) };

        if (!string.IsNullOrWhiteSpace(area))
            filters.Add(fb.Eq(v => v.Area, area));

        if (skillsMust != null)
        {
            foreach (var s in skillsMust.Where(x => !string.IsNullOrWhiteSpace(x)))
                filters.Add(fb.AnyEq(v => v.MustSkills, s.ToLower().Trim()));
        }

        if (skillsNice != null && skillsNice.Any())
            filters.Add(fb.In(nameof(Vaga.NiceSkills), skillsNice.Select(s => s.ToLower().Trim())));

        var filter = filters.Count == 1 ? filters[0] : fb.And(filters);
        return await _collection.Find(filter).ToListAsync();
    }
}