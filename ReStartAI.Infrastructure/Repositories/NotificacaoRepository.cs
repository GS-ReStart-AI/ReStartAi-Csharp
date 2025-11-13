using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using ReStartAI.Infrastructure.Context;
using MongoDB.Driver;

namespace ReStartAI.Infrastructure.Repositories
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        private readonly IMongoCollection<Notificacao> _collection;

        public NotificacaoRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Notificacao>("Notificacoes");
        }

        public async Task<IEnumerable<Notificacao>> GetAllAsync(int page, int pageSize)
        {
            return await _collection.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<Notificacao?> GetByIdAsync(string id)
        {
            var filter = Builders<Notificacao>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Notificacao> CreateAsync(Notificacao entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, Notificacao entity)
        {
            var filter = Builders<Notificacao>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Notificacao>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<int> CountAsync()
        {
            return (int)await _collection.CountDocumentsAsync(_ => true);
        }
    }
}