using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ReStartAI.Domain.Entities;

namespace ReStartAI.Infrastructure.Data;

public class MongoOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Database { get; set; } = default!;
}

public class MongoContext
{
    public IMongoDatabase Database { get; }
    public IMongoCollection<Usuario> Usuarios => Database.GetCollection<Usuario>("usuarios");
    public IMongoCollection<Curriculo> Curriculos => Database.GetCollection<Curriculo>("curriculos");
    public IMongoCollection<Vaga> Vagas => Database.GetCollection<Vaga>("vagas");
    public IMongoCollection<Candidatura> Candidaturas => Database.GetCollection<Candidatura>("candidaturas");
    public IMongoCollection<Notification> Notifications => Database.GetCollection<Notification>("notifications");

    public MongoContext(IConfiguration configuration)
    {
        var conn = configuration["Mongo:ConnectionString"]!;
        var dbName = configuration["Mongo:Database"] ?? "restartai";
        var client = new MongoClient(conn);
        Database = client.GetDatabase(dbName);
    }
}