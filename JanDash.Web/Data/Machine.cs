using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace JanDash.Data
{
    [BsonIgnoreExtraElements]
    public class Machine
    {
        public string Name { get; set; }
        public string MachineId { get; set; }
        public string OwnerId { get; set; }
        public MachineInfo Info { get; set; }
        public string Token { get; set; }
        public MachineMemoryInfo MemoryInfo { get; set; }
        public MachineOtherInfo OtherInfo { get; set; }
        [JsonIgnore] [BsonIgnore] public List<Func<Task>> Changed { get; } = new();
        [BsonIgnore] public long LastContact { get; private set; }

        public Machine()
        {
        }

        public Machine(string name, string ownerId)
        {
            (Name, OwnerId, MachineId, Info, Token, MemoryInfo, OtherInfo) =
                (name, ownerId, Ulid.NewUlid().ToString(), new(), AuthToken.Generate(), new(), new());
        }

        public Task UpdateAsync()
            => Mongo.MachinesCollection.ReplaceOneAsync(
                new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument("MachineId", MachineId)),
                this.ToBsonDocument());

        /// <summary>
        /// Starts a Task to run all the Changed handlers
        /// </summary>
        public void Updated()
        {
            LastContact = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Task.Factory.StartNew(() =>
            {
                foreach (var handler in Changed)
                {
                    handler();
                }
            });
        }
    }
}