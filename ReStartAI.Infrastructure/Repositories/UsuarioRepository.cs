using MongoDB.Driver;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Data;

namespace ReStartAI.Infrastructure.Repositories;

public class UsuarioRepository : MongoRepository<Usuario>, IUsuarioRepository
{
    private readonly IMongoCollection<Usuario> _collection;

    public UsuarioRepository(MongoContext context) : base(context.Usuarios)
    {
        _collection = context.Usuarios;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _collection.Find(x => x.Email == email).AnyAsync();
    }
}