using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services;
using System;
using System.Threading.Tasks;

namespace OtokatariBackend.Persistence.MongoDB.DAO.SystemLibrary
{
    public class MusicLibraryOperator : IOtokatariDbOperator
    {
        private readonly MongoClient _client;
        private readonly MongoContext _context;
        private readonly ILogger<MusicLibraryOperator> _logger;
        private readonly static object MusicLibraryLocker = new object();

        public MusicLibraryOperator(MongoClient client, MongoContext context, ILogger<MusicLibraryOperator> logger)
        {
            _client = client;
            _context = context;
            _logger = logger;
        }

        public bool AppendMusicToLibraryIfNotExist(SimpleMusic music)
        {
            lock (MusicLibraryLocker)
            {
                if (!IfMusicExists(music.Musicid))
                {
                    return SaveNewMusicRecord(music.ToMusicLibrary());
                }
                return true;
            }
        }

        public async Task<bool> SaveNewMusicRecordAsync(SystemMusicLibrary musicLibrary)
        {
            try
            {
                await _context.SystemMusicLibrary.InsertOneAsync(musicLibrary);
                return musicLibrary.id != null;
            }
            catch (Exception e)
            {
                _logger.LogError($"Database update mistake! {e.Message}");
            }
            return false;
        }

        public bool SaveNewMusicRecord(SystemMusicLibrary musicLibrary)
        {
            try
            {
                _context.SystemMusicLibrary.InsertOne(musicLibrary);
                return musicLibrary.id != null;
            }
            catch (Exception e)
            {
                _logger.LogError($"Database update mistake! {e.Message}");
            }
            return false;
        }

        public bool IfMusicExists(string Musicid)
        {
            return 0 < _context.SystemMusicLibrary.CountDocuments(x => x.Musicid == Musicid);
        }
    }
}
