using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities
{
    public class Vaga
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("empresa")]
        public string Empresa { get; set; } = string.Empty;

        [BsonElement("cidade")]
        public string Cidade { get; set; } = string.Empty;

        [BsonElement("senioridade")]
        public string Senioridade { get; set; } = string.Empty;

        [BsonElement("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [BsonElement("reqMust")]
        public string ReqMust { get; set; } = string.Empty;

        [BsonElement("reqNice")]
        public string ReqNice { get; set; } = string.Empty;

        [BsonElement("ativa")]
        public bool Ativa { get; set; }

        [BsonIgnore]
        public List<string> MustSkills => SplitSkills(ReqMust);

        [BsonIgnore]
        public List<string> NiceSkills => SplitSkills(ReqNice);

        private static List<string> SplitSkills(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            return input
                .Split(',', ';', '|', '\n')
                .Select(s => s.Trim().ToLowerInvariant())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();
        }
    }
}