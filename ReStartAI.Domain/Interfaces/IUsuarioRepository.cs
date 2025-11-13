using ReStartAI.Domain.Entities;

namespace ReStartAI.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync(int page, int pageSize);
        Task<Usuario?> GetByIdAsync(string id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario> CreateAsync(Usuario entity);
        Task UpdateAsync(string id, Usuario entity);
        Task DeleteAsync(string id);
        Task<int> CountAsync();
    }
}