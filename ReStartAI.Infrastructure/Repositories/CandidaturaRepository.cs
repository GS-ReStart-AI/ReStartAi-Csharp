using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using ReStartAI.Infrastructure.Context;
using MongoDB.Driver;

namespace ReStartAI.Infrastructure.Repositories
{
    public class CandidaturaRepository : ICandidaturaRepository
    {
        private readonly IMongoCollection<Candidatura> _collection;

        public CandidaturaRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Candidatura>("Candidaturas");
        }

        public async Task<IEnumerable<Candidatura>> GetAllAsync(int page, int pageSize)
        {
            return await _collection.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<Candidatura?> GetByIdAsync(string id)
        {
            var filter = Builders<Candidatura>.Filter.Eq(c => c.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Candidatura> CreateAsync(Candidatura entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, Candidatura entity)
        {
            var filter = Builders<Candidatura>.Filter.Eq(c => c.Id, id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Candidatura>.Filter.Eq(c => c.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<int> CountAsync()
        {
            return (int)await _collection.CountDocumentsAsync(_ => true);
        }
    }
}