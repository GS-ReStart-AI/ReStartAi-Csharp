using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace ReStartAI.Infrastructure.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoSettings:ConnectionString"]);
            _database = client.GetDatabase(configuration["MongoSettings:DatabaseName"]);
        }

        public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);
    }
}