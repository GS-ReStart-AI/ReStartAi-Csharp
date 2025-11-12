using ReStartAI.Domain.Entities;

namespace ReStartAI.Infrastructure.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetByUserAsync(string userId, bool? lido = null);
    Task MarkAsReadAsync(string id);
    Task MarkAllAsReadAsync(string userId);
}