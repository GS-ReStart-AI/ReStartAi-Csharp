using MongoDB.Driver;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using ReStartAI.Infrastructure.Context;

namespace ReStartAI.Infrastructure.Repositories
{
    public class AppEventRepository : IAppEventRepository
    {
        private readonly IMongoCollection<AppEvent> _collection;

        public AppEventRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<AppEvent>("appevents");
        }

        public async Task<IEnumerable<AppEvent>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _collection.Find(_ => true)
                .SortByDescending(e => e.TimestampUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<AppEvent?> GetByIdAsync(string id)
        {
            var filter = Builders<AppEvent>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<AppEvent> CreateAsync(AppEvent entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, AppEvent entity)
        {
            var filter = Builders<AppEvent>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<AppEvent>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}