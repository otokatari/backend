using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Persistence.MongoDB.DAO.Sharing;
using OtokatariBackend.Persistence.MongoDB.Model;

namespace OtokatariBackend.Services.Music
{
    public class SharingServices : IOtokatariService
    {
        private readonly SharingDbOperator _sharing;
        private readonly ILogger<SharingServices> _logger;

        public SharingServices(SharingDbOperator sharing, ILogger<SharingServices> logger)
        {
            _sharing = sharing;
            _logger = logger;
        }

        public async Task<object> GetMusicSharingComments(string Musicid)
        {
            var comments = await _sharing.GetMusicCommentsAsync(Musicid);
            if (comments != null)
            {
                return new
                {
                    StatusCode = 0,
                    Comments = comments
                };
            }
            return new CommonResponse { StatusCode = -1 };
        }


        public async Task<CommonResponse> PostSharingComment(string Musicid, SharingComments comments)
        {
            comments._id = ObjectId.GenerateNewId();
            comments.Comments = new ReplyComments[0]; // 默认没有回复，避免客户端忘了初始化值。
            comments.Like = new string[0];
            var result = await _sharing.CreateMusicSharingComment(Musicid, comments);
            return new CommonResponse { StatusCode = result ? 0 : -1 };
        }

        public async Task<CommonResponse> ReplySharingComment(string Musicid, string Commentid, ReplyComments comments)
        {
            var result = await _sharing.ReplyMusicSharingComment(Musicid, ObjectId.Parse(Commentid), comments);
            return new CommonResponse { StatusCode = result ? 0 : -1 };
        }
        public async Task<CommonResponse> LikeComment(string Musicid, string Commentid, string Userid)
        {
            var commentid = ObjectId.Parse(Commentid);
            var result = await _sharing.LikeOneComment(Musicid, Userid, commentid);
            return new CommonResponse { StatusCode = result ? 0 : -1 };
        }
        public async Task<CommonResponse> UnLikeComment(string Musicid, string Commentid, string Userid)
        {
            var commentid = ObjectId.Parse(Commentid);
            var result = await _sharing.UnLikeOneComment(Musicid, Userid, commentid);
            return new CommonResponse { StatusCode = result ? 0 : -1 };
        }
    }
}