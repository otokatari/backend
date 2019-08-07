using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services;

namespace OtokatariBackend.Persistence.MongoDB.DAO.DbSingers
{
    public class SingersDbOperator : IOtokatariDbOperator
    {
        private readonly MongoContext _context;

        private readonly ILogger<SingersDbOperator> _logger;
        public SingersDbOperator(MongoContext context, ILogger<SingersDbOperator> logger)
        {
            _context = context;
            _logger = logger;
        }

        public UserSavedSingerList<Singers> GetUserSavedSingerList(string Userid)
        {
            var singerList = _context.UserSavedSingerList
                                        .Aggregate()
                                        .Match($"{{ Userid: \"{Userid}\" }}")
                                        .Lookup("Singers", "SavedList", "_id", "SavedList")
                                        .Lookup("Singers", "SystemList", "_id", "SystemList")
                                        .FirstOrDefault();

            return singerList != null ? BsonSerializer.Deserialize<UserSavedSingerList<Singers>>(singerList) : null;
        }

        public async Task<bool> AddLikeSinger(string Userid, ObjectId SingerObjId)
        {
            var pusher = Builders<UserSavedSingerList<ObjectId>>.Update.Push(r => r.SavedList, SingerObjId);
            var userFilter = Builders<UserSavedSingerList<ObjectId>>.Filter.Eq(r => r.Userid, Userid);

            var updateResult = await _context.UserSavedSingerList.UpdateOneAsync(userFilter, pusher, new UpdateOptions { IsUpsert = true });
            return updateResult.ModifiedCount == 1 || updateResult.UpsertedId != null;
        }

        public async Task<bool> DeleteLikeSinger(string Userid,ObjectId SingerObjId)
        {
            var userFilter = Builders<UserSavedSingerList<ObjectId>>.Filter.Eq(r => r.Userid, Userid);
            var puller = Builders<UserSavedSingerList<ObjectId>>.Update.Pull(r => r.SavedList, SingerObjId);

            var session = await _context._client.StartSessionAsync();
            session.StartTransaction();
            var deleteResult = await _context.UserSavedSingerList.UpdateOneAsync(userFilter,puller);

            var status = deleteResult.ModifiedCount == 1;

            if(!status) await session.AbortTransactionAsync();
            else await session.CommitTransactionAsync();
            
            return status;
        }
    }
}