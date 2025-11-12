using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities;

public class Curriculo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = default!;

    [BsonElement("arquivoNome")]
    public string? ArquivoNome { get; set; }

    [BsonElement("textoOriginal")]
    public string? TextoOriginal { get; set; }

    [BsonElement("textoExtraido")]
    public string? TextoExtraido { get; set; }

    [BsonElement("skills")]
    public List<string> Skills { get; set; } = new();

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}