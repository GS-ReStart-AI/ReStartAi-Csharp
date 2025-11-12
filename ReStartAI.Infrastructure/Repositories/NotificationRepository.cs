using MongoDB.Bson;
using MongoDB.Driver;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Data;

namespace ReStartAI.Infrastructure.Repositories;

public class NotificationRepository : MongoRepository<Notification>, INotificationRepository
{
    private readonly IMongoCollection<Notification> _collection;

    public NotificationRepository(MongoContext context) : base(context.Notifications)
    {
        _collection = context.Notifications;
    }

    public async Task<List<Notification>> GetByUserAsync(string userId, bool? lido = null)
    {
        var filter = Builders<Notification>.Filter.Eq(x => x.UserId, userId);
        if (lido.HasValue)
            filter = Builders<Notification>.Filter.And(filter, Builders<Notification>.Filter.Eq(x => x.Lido, lido.Value));
        return await _collection.Find(filter).SortByDescending(x => x.CriadoEm).ToListAsync();
    }

    public async Task MarkAsReadAsync(string id)
    {
        var filter = Builders<Notification>.Filter.Eq("_id", ObjectId.Parse(id));
        var update = Builders<Notification>.Update.Set(x => x.Lido, true);
        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var filter = Builders<Notification>.Filter.Eq(x => x.UserId, userId);
        var update = Builders<Notification>.Update.Set(x => x.Lido, true);
        await _collection.UpdateManyAsync(filter, update);
    }
}