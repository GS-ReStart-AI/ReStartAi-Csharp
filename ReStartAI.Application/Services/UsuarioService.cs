using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Application.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Usuario>> GetAllAsync(int page, int pageSize) => _repository.GetAllAsync(page, pageSize);
        public Task<Usuario?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
        public Task<Usuario> CreateAsync(Usuario entity) => _repository.CreateAsync(entity);
        public Task UpdateAsync(string id, Usuario entity) => _repository.UpdateAsync(id, entity);
        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
    }
}