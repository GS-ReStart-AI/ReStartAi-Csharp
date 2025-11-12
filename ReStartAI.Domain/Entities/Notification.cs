using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = default!;

    [BsonElement("titulo")]
    public string Titulo { get; set; } = default!;

    [BsonElement("mensagem")]
    public string Mensagem { get; set; } = default!;

    [BsonElement("lido")]
    public bool Lido { get; set; } = false;

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}