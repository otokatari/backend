using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services;
using OtokatariBackend.Utils.TypeMerger;

namespace OtokatariBackend.Persistence.MongoDB.DAO.DbSingers
{
    public class SingersDbOperator : IOtokatariDbOperator
    {
        private readonly MongoContext _context;
        private readonly ILogger<SingersDbOperator> _logger;

        private readonly object InsertNewSingerLock = new object();
        public SingersDbOperator(MongoContext context, ILogger<SingersDbOperator> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void InsertNewSingerInfoToDbIfNotExists(string Singerid,string Platform, string SingerName)
        {
            var singer = new Singers
            {
                SingerName = SingerName.Trim()
            };
            switch (Platform)
            {
                case "netease":
                    {
                        if (string.IsNullOrEmpty(singer.NeteaseId))
                            singer.NeteaseId = Singerid;
                        break;
                    }
                case "kugou":
                    {
                        if (string.IsNullOrEmpty(singer.NeteaseId))
                            singer.KugouId = Singerid;
                        break;
                    }
                case "qqmusic":
                    {
                        if (string.IsNullOrEmpty(singer.NeteaseId))
                            singer.QQMusicId = Singerid;
                        break;
                    }
            }
            UpdateSingerInfo(singer);
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


        public Singers GetSingerInfoByObjectId(ObjectId SingerObjid) 
                        => GetSingerInfo(f => f.Eq(x => x._id, SingerObjid));
        public Singers GetSingerInfoByPlatformId(string Singerid) 
                        => GetSingerInfo(f => f.Or((new[] { "NeteaseId", "KugouId", "QQMusicId"  }).Select(y => f.Eq(y, Singerid))));
        public Singers GetSingerInfoByName(string SingerName) 
                        => GetSingerInfo(f => f.Eq(x => x.SingerName, SingerName));

        private Singers GetSingerInfo(Func<FilterDefinitionBuilder<Singers>,FilterDefinition<Singers>> filterFunc)
        {
            var filter = Builders<Singers>.Filter;
            var preparedFilter = filterFunc(filter);
            return _context.Singers.Find(preparedFilter).FirstOrDefault();
        }

        public bool UpdateSingerInfo(Singers singerNewInfo)
        {
            var filter = Builders<Singers>.Filter;
            var singerFilter = filter.Eq(r => r.SingerName,singerNewInfo.SingerName);

            var updater = GetSingerUpdater(singerNewInfo);
            var session = _context._client.StartSession();
            session.StartTransaction();
            var result = _context.Singers.UpdateOne(singerFilter, updater, new UpdateOptions { IsUpsert = true });
            if(result.MatchedCount == 1 && result.ModifiedCount == 0 && result.UpsertedId == null)
            {
                // 不需要改动
                _logger.LogInformation($"歌手信息: {singerNewInfo.SingerName} 不需要更新.");
                session.CommitTransaction();
                return true;
            }
            else if(result.ModifiedCount == 1 || result.UpsertedId != null)
            {
                // 满足条件commit.
                _logger.LogInformation($"歌手信息: {singerNewInfo.SingerName} 成功更新.");
                session.CommitTransaction();
                return true;
            }
            // 其他条件rollback
            _logger.LogInformation($"歌手信息: {singerNewInfo.SingerName} 错误回滚: {result}.");
            session.AbortTransaction();
            return false;
        }

        private List<string> SingerKnownPlatformIds(Singers s) 
                        => (new[] { s.NeteaseId, s.KugouId, s.QQMusicId }).Where(x => !string.IsNullOrEmpty(x)).ToList();

        private UpdateDefinition<Singers> GetSingerUpdater(Singers s)
        {
            var u = Builders<Singers>.Update;
            var updatePipe = new List<UpdateDefinition<Singers>>();
            if (!string.IsNullOrEmpty(s.SingerName))
                updatePipe.Add(u.Set(r => r.SingerName, s.SingerName));
            if (!string.IsNullOrEmpty(s.NeteaseId))
                updatePipe.Add(u.Set(r => r.NeteaseId, s.NeteaseId));
            if (!string.IsNullOrEmpty(s.KugouId))
                updatePipe.Add(u.Set(r => r.KugouId, s.KugouId));
            if (!string.IsNullOrEmpty(s.QQMusicId))
                updatePipe.Add(u.Set(r => r.QQMusicId, s.QQMusicId));
            if (s.Language != null)
                updatePipe.Add(u.Set(r => r.Language, s.Language));
            return u.Combine(updatePipe);
        }
    }
}