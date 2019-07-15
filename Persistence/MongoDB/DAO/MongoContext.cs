using MongoDB.Driver;
using OtokatariBackend.Persistence.MongoDB.Model;
using System.Threading.Tasks;

namespace OtokatariBackend.Persistence.MongoDB.DAO
{
    public class MongoContext
    {
        public readonly IMongoCollection<MusicComments> MusicComments;
        public readonly IMongoCollection<Playlists> Playlists;
        public readonly IMongoCollection<SystemMusicLibrary> SystemMusicLibrary;
        public readonly IMongoCollection<UserBehaviour> UserBehaviour;
        public readonly IMongoDatabase db;
        public MongoContext(IMongoDatabase _client)
        {
            db = _client;
            
            MusicComments = db.GetCollection<MusicComments>("MusicComments", new MongoCollectionSettings { AssignIdOnInsert = true });
            Playlists = db.GetCollection<Playlists>("Playlists", new MongoCollectionSettings { AssignIdOnInsert = true });
            SystemMusicLibrary = db.GetCollection<SystemMusicLibrary>("SystemMusicLibrary", new MongoCollectionSettings { AssignIdOnInsert = true });
            UserBehaviour = db.GetCollection<UserBehaviour>("UserBehaviour", new MongoCollectionSettings { AssignIdOnInsert = true });
        }

    }

    public static class MongoClientExtension
    {
        public static async Task<IClientSessionHandle> StartStrictTransactionAsync(this MongoClient client)
        {
            return await client.StartSessionAsync(new ClientSessionOptions { CausalConsistency = false });
        }
    }
}
