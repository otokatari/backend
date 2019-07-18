using MongoDB.Bson;
using MongoDB.Driver;
using OtokatariBackend.Persistence.MongoDB.DAO.SystemLibrary;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services;
using OtokatariBackend.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Persistence.MongoDB.DAO.Playlist
{
    public class PlaylistOperator : IOtokatariDbOperator
    {
        private readonly MongoContext _context;
        private readonly MusicLibraryOperator _musicLib;
        private readonly MongoClient _client;
        public PlaylistOperator(MongoContext context, MusicLibraryOperator musicLib, MongoClient client)
        {
            _context = context;
            _musicLib = musicLib;
            _client = client;
        }

        public IQueryable<Playlists> QueryPlaylists(string UserID)
        {
            return _context.Playlists.AsQueryable().Where(x => x.Userid == UserID);
        }

        public bool IfMusicExistsInPlaylist(ObjectId PlaylistId, string MusicId)
        {
            return _context.Playlists.AsQueryable().FirstOrDefault(x => x._id == PlaylistId)?.Songs.Count(x => x.Musicid == MusicId) > 0;
        }

        public Playlists QuerySelectedPlaylist(ObjectId PlaylistId) => _context.Playlists.AsQueryable().FirstOrDefault(x => x._id == PlaylistId);

        public async Task<bool> AddSong(Playlists list, PlaylistMusic song)
        {
            if (list != null)
            {
                var update = await _context.Playlists.UpdateOneAsync(new BsonDocument("_id", list._id),
                                                  Builders<Playlists>.Update.Push("songs",song));
                if (update.ModifiedCount == 1)
                {
                    return true;
                }
                return false;
            }
            return false;
        }


        public async Task<bool> DeleteSong(ObjectId PlaylistObjectId, string Userid, string Musicid)
        {
            var list = _context.Playlists.AsQueryable().FirstOrDefault(x => x._id == PlaylistObjectId);
            if (list != null)
            {
                if (Userid != list.Userid) return false; // 不允许删除别人的歌曲

                var filterSongs = Builders<PlaylistMusic>.Filter.Eq(r => r.Musicid,Musicid);
                var pull = Builders<Playlists>.Update.PullFilter(r => r.Songs,filterSongs);

                var update = await _context.Playlists.UpdateOneAsync(new BsonDocument("_id", PlaylistObjectId),pull);
                if (update.ModifiedCount == 1)
                {
                    return true;
                }
                return false;
            }
            return false;
        }


        public async Task<bool> DeletePlaylist(ObjectId PlaylistObjectId, string Userid)
        {
            return null != await _context.Playlists.FindOneAndDeleteAsync(new BsonDocument("_id", PlaylistObjectId).Add("userid", Userid));
        }

        public async Task<Playlists> CreatePlaylist(string PlaylistName, string UserId, bool IsDefaultFavourite)
        {
            var list = new Playlists
            {
                Userid = UserId,
                Name = PlaylistName,
                Favourite = IsDefaultFavourite,
                CreateTime = (int)DateUtil.DateToUnix(DateTime.Now),
                Songs = new PlaylistMusic[0]
            };
            await _context.Playlists.InsertOneAsync(list);
            return list._id != null ? list : null;
        }
    }
}

