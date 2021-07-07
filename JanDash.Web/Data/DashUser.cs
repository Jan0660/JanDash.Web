using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace JanDash.Data
{
    [BsonIgnoreExtraElements]
    public class DashUser
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public List<string> Tokens { get; set; }

        public DashUser()
        {
        }

        public DashUser(string username, string password)
        {
            (Username, Tokens, UserId, Password) = (username, new(), Ulid.NewUlid().ToString(),
                SecurePasswordHasher.Hash(password));
        }

        public Task UpdateAsync()
            => Mongo.UsersCollection.ReplaceOneAsync(
                new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument("UserId", UserId)),
                this.ToBsonDocument());
    }
}