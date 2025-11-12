using ReStartAI.Domain.Entities;

namespace ReStartAI.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync(int pageNumber, int pageSize);
        Task<Usuario?> GetByIdAsync(string id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario> CreateAsync(Usuario entity);
        Task UpdateAsync(string id, Usuario entity);
        Task DeleteAsync(string id);
    }
}