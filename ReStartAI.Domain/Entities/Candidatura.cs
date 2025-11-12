using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities
{
    public class Candidatura
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("usuarioId")]
        public string UsuarioId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("vagaId")]
        public string VagaId { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = string.Empty;

        [BsonElement("scoreMatch")]
        public int ScoreMatch { get; set; }

        [BsonElement("applyUrl")]
        public string ApplyUrl { get; set; } = string.Empty;
    }
}