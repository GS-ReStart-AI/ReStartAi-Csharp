using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities;

public class Candidatura
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = default!;

    [BsonElement("vagaId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string VagaId { get; set; } = default!;

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}