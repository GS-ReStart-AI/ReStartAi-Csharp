using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReStartAI.Domain.Entities;

public class AppEvent
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = default!;

    [BsonElement("type")]
    public string Type { get; set; } = default!;

    [BsonElement("timestampUtc")]
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    [BsonElement("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}