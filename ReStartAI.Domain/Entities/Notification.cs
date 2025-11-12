using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities
{
    public class Notificacao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("usuarioId")]
        public string UsuarioId { get; set; } = string.Empty;

        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("mensagem")]
        public string Mensagem { get; set; } = string.Empty;
    }
}