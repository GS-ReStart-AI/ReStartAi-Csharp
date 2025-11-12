using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Application.Services
{
    public class VagaService
    {
        private readonly IVagaRepository _repository;

        public VagaService(IVagaRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Vaga>> GetAllAsync(int page, int pageSize) => _repository.GetAllAsync(page, pageSize);
        public Task<Vaga?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
        public Task<Vaga> CreateAsync(Vaga entity) => _repository.CreateAsync(entity);
        public Task UpdateAsync(string id, Vaga entity) => _repository.UpdateAsync(id, entity);
        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
    }
}