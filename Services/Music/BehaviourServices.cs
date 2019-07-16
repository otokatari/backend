using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Persistence.MongoDB.DAO;
using OtokatariBackend.Persistence.MongoDB.Model;
using System.Threading.Tasks;
using OtokatariBackend.Persistence.MongoDB.DAO.SystemLibrary;

namespace OtokatariBackend.Services.Music
{
    public class BehaviourServices : IOtokatariService
    {
        private readonly ILogger<BehaviourServices> _logger;
        private readonly MongoContext _context;
        private readonly MusicLibraryOperator _musiclib;


        public BehaviourServices(MongoContext context, MusicLibraryOperator musiclib, ILogger<BehaviourServices> logger)
        {
            _context = context;
            _logger = logger;
            _musiclib = musiclib;
        }

        public async Task<CommonResponse> ReportBehavior(UserBehaviour behaviour)
        {
            // 提取出本次上报播放的歌曲信息
            _ = Task.Run(() => _musiclib.AppendMusicToLibraryIfNotExist(behaviour.Music));

            await _context.UserBehaviour.InsertOneAsync(behaviour);

            return new CommonResponse { StatusCode = 0 };
        }

    }
}
