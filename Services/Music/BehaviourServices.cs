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
            var music = behaviour.Music;
            if (!await _musiclib.IfMusicExists(music.Musicid))
            {
                if(await _musiclib.SaveNewMusicRecord(music.ToMusicLibrary()))
                {
                    await _context.UserBehaviour.InsertOneAsync(behaviour);
                    return new CommonResponse { StatusCode = 1 };
                }
                else return new CommonResponse { StatusCode = -1 }; // 保存新歌不成功, 返回错误信息, 提示客户端重新上报保存
            }
            await _context.UserBehaviour.InsertOneAsync(behaviour);
            return new CommonResponse { StatusCode = 0 };
        }
    }
}
