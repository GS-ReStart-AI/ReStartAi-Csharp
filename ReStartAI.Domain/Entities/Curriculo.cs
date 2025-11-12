using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities
{
    public class Curriculo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("usuarioId")]
        public string UsuarioId { get; set; } = string.Empty;

        [BsonElement("nomeArquivo")]
        public string NomeArquivo { get; set; } = string.Empty;

        [BsonElement("texto")]
        public string Texto { get; set; } = string.Empty;

        [BsonElement("skills")]
        public List<string> Skills { get; set; } = new();

        [BsonElement("criadoEm")]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}