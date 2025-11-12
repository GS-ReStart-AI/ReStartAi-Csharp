using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace ReStartAI.Infrastructure.Repositories;

public class MongoRepository<T> : IRepository<T>
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoCollection<T> collection)
    {
        _collection = collection;
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<T> InsertAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false });
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await _collection.DeleteOneAsync(filter);
    }
}