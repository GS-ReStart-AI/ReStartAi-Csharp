using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using ReStartAI.Infrastructure.Context;
using MongoDB.Driver;

namespace ReStartAI.Infrastructure.Repositories
{
    public class VagaRepository : IVagaRepository
    {
        private readonly IMongoCollection<Vaga> _collection;

        public VagaRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Vaga>("Vagas");
        }

        public async Task<IEnumerable<Vaga>> GetAllAsync(int page, int pageSize)
        {
            return await _collection.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<Vaga?> GetByIdAsync(string id)
        {
            var filter = Builders<Vaga>.Filter.Eq(v => v.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Vaga> CreateAsync(Vaga entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, Vaga entity)
        {
            var filter = Builders<Vaga>.Filter.Eq(v => v.Id, id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Vaga>.Filter.Eq(v => v.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<int> CountAsync()
        {
            return (int)await _collection.CountDocumentsAsync(_ => true);
        }
    }
}