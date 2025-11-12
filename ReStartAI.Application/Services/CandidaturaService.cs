using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Application.Services
{
    public class CandidaturaService
    {
        private readonly ICandidaturaRepository _repository;

        public CandidaturaService(ICandidaturaRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Candidatura>> GetAllAsync(int page, int pageSize) => _repository.GetAllAsync(page, pageSize);
        public Task<Candidatura?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
        public Task<Candidatura> CreateAsync(Candidatura entity) => _repository.CreateAsync(entity);
        public Task UpdateAsync(string id, Candidatura entity) => _repository.UpdateAsync(id, entity);
        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
    }
}