using ReStartAI.Domain.Entities;

namespace ReStartAI.Infrastructure.Repositories;

public interface IAppEventRepository : IRepository<AppEvent>
{
    Task InsertAsync(AppEvent e);
    Task<List<AppEvent>> GetLastAsync(string userId, int limit = 10);
    Task<(int jobsViewedToday, int applyClicksToday, DateTime? lastEventAt)> GetTodayMetricsAsync(string userId);
}