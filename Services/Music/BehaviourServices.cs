using Microsoft.Extensions.Logging;
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
        private readonly AnalyzerQueue _queue;

        public BehaviourServices(MongoContext context,
                                MusicLibraryOperator musiclib,
                                ILogger<BehaviourServices> logger,
                                AnalyzerQueue queue)
        {
            _context = context;
            _logger = logger;
            _musiclib = musiclib;
            _queue = queue;
        }

        public async Task<CommonResponse> ReportBehavior(UserBehaviour behaviour)
        {
            // 提取出本次上报播放的歌曲信息
            _ = Task.Run(() =>
            {
                var NeedAnalysis = _musiclib.AppendMusicToLibraryIfNotExist(behaviour.Music);
                // 如果这是一首服务器上没有的新歌，就需要进行分析.
                
                if(NeedAnalysis)
                     _queue.SendAnalyzeMusicMessage(behaviour.Music);
            });

            await _context.UserBehaviour.InsertOneAsync(behaviour);

            return new CommonResponse { StatusCode = 0 };
        }

    }
}
