using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities;

public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nomeCompleto")]
    public string NomeCompleto { get; set; } = default!;

    [BsonElement("cpf")]
    public string Cpf { get; set; } = default!;

    [BsonElement("dataNascimento")]
    public DateTime? DataNascimento { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = default!;

    [BsonElement("senhaHash")]
    public string SenhaHash { get; set; } = default!;

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    [BsonElement("atualizadoEm")]
    public DateTime? AtualizadoEm { get; set; }
}