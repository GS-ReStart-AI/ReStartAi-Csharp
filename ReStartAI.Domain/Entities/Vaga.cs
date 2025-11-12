using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities;

public class Vaga
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("titulo")]
    public string Titulo { get; set; } = default!;

    [BsonElement("mustSkills")]
    public List<string> MustSkills { get; set; } = new();

    [BsonElement("niceSkills")]
    public List<string> NiceSkills { get; set; } = new();

    [BsonElement("area")]
    public string Area { get; set; } = default!;

    [BsonElement("ativo")]
    public bool Ativo { get; set; } = true;

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}