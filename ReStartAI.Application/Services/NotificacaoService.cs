using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Application.Services
{
    public class NotificacaoService
    {
        private readonly INotificacaoRepository _repository;

        public NotificacaoService(INotificacaoRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Notificacao>> GetAllAsync(int page, int pageSize) => _repository.GetAllAsync(page, pageSize);
        public Task<Notificacao?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
        public Task<Notificacao> CreateAsync(Notificacao entity) => _repository.CreateAsync(entity);
        public Task UpdateAsync(string id, Notificacao entity) => _repository.UpdateAsync(id, entity);
        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
    }
}