using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Application.Services
{
    public class BaseService<T>
    {
        private readonly IRepository<T> _repository;

        public BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize) => _repository.GetAllAsync(pageNumber, pageSize);
        public Task<T?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
        public Task<T> CreateAsync(T entity) => _repository.CreateAsync(entity);
        public Task UpdateAsync(string id, T entity) => _repository.UpdateAsync(id, entity);
        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
    }
}