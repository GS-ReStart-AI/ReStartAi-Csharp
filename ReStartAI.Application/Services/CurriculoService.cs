using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Application.Services
{
    public class CurriculoService
    {
        private readonly ICurriculoRepository _repository;

        public CurriculoService(ICurriculoRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Curriculo>> GetAllAsync(int page, int pageSize) => _repository.GetAllAsync(page, pageSize);
        public Task<Curriculo?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
        public Task<Curriculo> CreateAsync(Curriculo entity) => _repository.CreateAsync(entity);
        public Task UpdateAsync(string id, Curriculo entity) => _repository.UpdateAsync(id, entity);
        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
    }
}