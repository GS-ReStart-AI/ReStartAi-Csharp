using ReStartAI.Domain.Entities;

namespace ReStartAI.Infrastructure.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}