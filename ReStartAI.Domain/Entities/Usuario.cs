using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("nomeCompleto")]
        public string NomeCompleto { get; set; } = string.Empty;

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [BsonElement("dataNascimento")]
        public DateTime DataNascimento { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("senhaHash")]
        public string SenhaHash { get; set; } = string.Empty;
    }
}