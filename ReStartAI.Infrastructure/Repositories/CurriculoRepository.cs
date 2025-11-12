using MongoDB.Driver;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using ReStartAI.Infrastructure.Context;

namespace ReStartAI.Infrastructure.Repositories
{
    public class CurriculoRepository : ICurriculoRepository
    {
        private readonly IMongoCollection<Curriculo> _collection;

        public CurriculoRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Curriculo>("curriculos");
        }

        public async Task<IEnumerable<Curriculo>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _collection.Find(_ => true)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<Curriculo?> GetByIdAsync(string id)
        {
            var filter = Builders<Curriculo>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Curriculo> CreateAsync(Curriculo entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, Curriculo entity)
        {
            var filter = Builders<Curriculo>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Curriculo>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}