using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Application.Services
{
    public class AppEventService
    {
        private readonly IAppEventRepository _repository;

        public AppEventService(IAppEventRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<AppEvent>> GetAllAsync(int page, int pageSize) => _repository.GetAllAsync(page, pageSize);
        public Task<AppEvent?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
        public Task<AppEvent> CreateAsync(AppEvent entity) => _repository.CreateAsync(entity);
        public Task UpdateAsync(string id, AppEvent entity) => _repository.UpdateAsync(id, entity);
        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
    }
}