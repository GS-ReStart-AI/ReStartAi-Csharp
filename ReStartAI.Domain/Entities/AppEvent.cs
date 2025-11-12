using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities
{
    public class AppEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("usuarioId")]
        public string UsuarioId { get; set; } = string.Empty;

        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [BsonElement("timestampUtc")]
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        [BsonElement("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }
    }
}