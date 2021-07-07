using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JanDash.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace JanDash
{
    public static class Mongo
    {
        public static MongoClient Client { get; private set; }
        public static IMongoDatabase Database { get; private set; }
        public static IMongoCollection<BsonDocument> UsersCollection { get; private set; }
        public static IMongoCollection<BsonDocument> MachinesCollection { get; private set; }

        public static List<Machine> MachinesStore { get; private set; }

        public static async Task Connect()
        {
            Client = new(Program.Config.MongoDbUrl);
            Database = Client.GetDatabase(Program.Config.DatabaseName);
            UsersCollection = Database.GetCollection<BsonDocument>("users");
            MachinesCollection = Database.GetCollection<BsonDocument>("machines");
            MachinesStore = new();
            Console.WriteLine("Loading machines...");
            await MachinesCollection.Find(new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument()))
                .ToListAsync()
                .ContinueWith(
                    res =>
                    {
                        Console.WriteLine("Received machines...");
                        MachinesStore = res.Result.ConvertAll(doc =>
                        {
                            var i = BsonSerializer.Deserialize<Machine>(doc);
                            i.PersistentInfo ??= new();
                            i.Info ??= new();
                            return i;
                        }).ToList();
                        Console.WriteLine("Loaded machines.");
                    });
        }

        public static DashUser GetUserByToken(string token)
            => BsonSerializer.Deserialize<DashUser>(
                UsersCollection.Find(
                        new JsonFilterDefinition<BsonDocument>($"{{ Tokens: {{ $all: [\"{token}\"] }} }}")).Limit(1)
                    .First());

        public static DashUser GetUserByUsername(string username)
            => BsonSerializer.Deserialize<DashUser>(UsersCollection
                .Find(new BsonDocument("Username", username)).FirstOrDefault());

        public static async Task<Machine> GetMachineByIdAsync(string id)
            => BsonSerializer.Deserialize<Machine>(
                await (UsersCollection.Find(
                        new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument("MachineId", id))).Limit(1)
                    .FirstAsync()));
    }
}