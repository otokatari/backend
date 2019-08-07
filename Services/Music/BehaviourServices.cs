using Microsoft.Extensions.Logging;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Persistence.MongoDB.DAO;
using OtokatariBackend.Persistence.MongoDB.Model;
using System.Threading.Tasks;
using OtokatariBackend.Persistence.MongoDB.DAO.SystemLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OtokatariBackend.Persistence.MongoDB.DAO.DbSingers;

namespace OtokatariBackend.Services.Music
{
    public class BehaviourServices : IOtokatariService
    {
        private readonly ILogger<BehaviourServices> _logger;
        private readonly MongoContext _context;
        private readonly MusicLibraryOperator _musiclib;

        private readonly SingersDbOperator _singers;
        private readonly AnalyzerQueue _queue;

        private static bool EnableMusicAnalyzer = false;

        public BehaviourServices(MongoContext context,
                                SingersDbOperator singers,
                                MusicLibraryOperator musiclib,
                                ILogger<BehaviourServices> logger,
                                AnalyzerQueue queue,
                                [FromServices] IConfiguration  config)
        {
            _context = context;
            _logger = logger;
            _musiclib = musiclib;
            _queue = queue;
            _singers = singers;
            EnableMusicAnalyzer = config.GetValue<bool>("EnableMusicAnalyzer");
            _logger.LogInformation($"MusicAnalyzer status: {EnableMusicAnalyzer}");
        }

        public async Task<CommonResponse> ReportBehavior(UserBehaviour behaviour)
        {
            // 提取出本次上报播放的歌曲信息
            _ = Task.Run(() =>
            {
                var NeedAnalysis = _musiclib.AppendMusicToLibraryIfNotExist(behaviour.Music);
                // 如果这是一首服务器上没有的新歌，就需要进行分析.
                var music = behaviour.Music;
                _singers.InsertNewSingerInfoToDbIfNotExists(music.Singerid,music.Platform,music.Singername);
                if(EnableMusicAnalyzer && NeedAnalysis)
                     _queue.SendAnalyzeMusicMessage(music);
            });

            await _context.UserBehaviour.InsertOneAsync(behaviour);

            return new CommonResponse { StatusCode = 0 };
        }

    }
}
