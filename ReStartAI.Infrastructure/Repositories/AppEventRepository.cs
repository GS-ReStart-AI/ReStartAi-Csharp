using MongoDB.Driver;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Data;

namespace ReStartAI.Infrastructure.Repositories;

public class AppEventRepository : MongoRepository<AppEvent>, IAppEventRepository
{
    private readonly IMongoCollection<AppEvent> _collection;

    public AppEventRepository(MongoContext context) : base(context.Database.GetCollection<AppEvent>("app_events"))
    {
        _collection = context.Database.GetCollection<AppEvent>("app_events");
    }

    public new async Task InsertAsync(AppEvent e)
    {
        await _collection.InsertOneAsync(e);
    }

    public async Task<List<AppEvent>> GetLastAsync(string userId, int limit = 10)
    {
        return await _collection
            .Find(x => x.UserId == userId)
            .SortByDescending(x => x.TimestampUtc)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<(int jobsViewedToday, int applyClicksToday, DateTime? lastEventAt)> GetTodayMetricsAsync(string userId)
    {
        var start = DateTime.UtcNow.Date;
        var end = start.AddDays(1);

        var filter = Builders<AppEvent>.Filter.And(
            Builders<AppEvent>.Filter.Eq(x => x.UserId, userId),
            Builders<AppEvent>.Filter.Gte(x => x.TimestampUtc, start),
            Builders<AppEvent>.Filter.Lt(x => x.TimestampUtc, end)
        );

        var list = await _collection.Find(filter).ToListAsync();

        var jobs = list.Count(x => x.Type == "view_job");
        var apply = list.Count(x => x.Type == "apply_click");
        DateTime? last = list.Count == 0 ? null : list.Max(x => x.TimestampUtc);

        return (jobs, apply, last);
    }
}