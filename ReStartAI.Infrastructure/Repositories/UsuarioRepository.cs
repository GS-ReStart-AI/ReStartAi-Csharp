using MongoDB.Driver;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using ReStartAI.Infrastructure.Context;

namespace ReStartAI.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _collection;

        public UsuarioRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<Usuario>("usuarios");
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _collection.Find(_ => true)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(string id)
        {
            var filter = Builders<Usuario>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            var filter = Builders<Usuario>.Filter.Eq(u => u.Email, email.ToLowerInvariant());
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Usuario> CreateAsync(Usuario entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, Usuario entity)
        {
            var filter = Builders<Usuario>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Usuario>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}