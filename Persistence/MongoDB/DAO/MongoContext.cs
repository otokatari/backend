using MongoDB.Bson;
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
        public readonly IMongoCollection<Singers> Singers;
        public readonly IMongoCollection<UserSavedSingerList<ObjectId>> UserSavedSingerList;
        
        public readonly IMongoDatabase _db;
        public readonly MongoClient _client;
        public MongoContext(IMongoDatabase db, MongoClient client)
        {
            _db = db;
            _client = client;
            var AssignIdOnInsert = new MongoCollectionSettings { AssignIdOnInsert = true };
            
            MusicComments = _db.GetCollection<MusicComments>("MusicComments", AssignIdOnInsert);
            Playlists = _db.GetCollection<Playlists>("Playlists", AssignIdOnInsert);
            SystemMusicLibrary = _db.GetCollection<SystemMusicLibrary>("SystemMusicLibrary", AssignIdOnInsert);
            UserBehaviour = _db.GetCollection<UserBehaviour>("UserBehaviour", AssignIdOnInsert);
            Singers = _db.GetCollection<Singers>("Singers", AssignIdOnInsert);
            UserSavedSingerList = _db.GetCollection<UserSavedSingerList<ObjectId>>("UserSavedSingerList", AssignIdOnInsert);
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
