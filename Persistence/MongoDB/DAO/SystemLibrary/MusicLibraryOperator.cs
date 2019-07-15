using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Persistence.MongoDB.DAO.SystemLibrary
{
    public class MusicLibraryOperator : IOtokatariDbOperator
    {
        private readonly MongoClient _client;
        private readonly MongoContext _context;
        private readonly ILogger<MusicLibraryOperator> _logger;

        public MusicLibraryOperator(MongoClient client, MongoContext context, ILogger<MusicLibraryOperator> logger)
        {
            _client = client;
            _context = context;
            _logger = logger;
        }

        public async Task<bool> SaveNewMusicRecord(SystemMusicLibrary musicLibrary)
        {
            //var trans = (await _client.StartStrictTransactionAsync());
            //trans.StartTransaction();
            try
            {
                await _context.SystemMusicLibrary.InsertOneAsync(musicLibrary);
                if (musicLibrary.id != null)
                {
                    // await trans.CommitTransactionAsync();
                    return true;
                }
                //await trans.AbortTransactionAsync();
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Database update mistake! {e.Message}");
                // await trans.AbortTransactionAsync();
            }
            return false;
        }

        public async Task<bool> IfMusicExists(string Musicid)
                                            => 0 < await _context.SystemMusicLibrary.CountDocumentsAsync(x => x.Musicid == Musicid);
    }
}
