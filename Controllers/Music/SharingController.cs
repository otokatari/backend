using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using OtokatariBackend.Persistence.MongoDB.DAO.Sharing;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services.Music;
using OtokatariBackend.Services.Token;
using OtokatariBackend.Utils;

namespace OtokatariBackend.Controllers.Music
{
    [Route("music/sharing")]
    [ApiController]
    public class SharingController : ControllerBase
    {
        private readonly StaticFilePathResovler _files;
        private readonly SharingDbOperator _sharing;
        private readonly ILogger<SharingController> _logger;
        public SharingController(SharingDbOperator sharing,
                                ILogger<SharingController> logger,
                                IOptions<StaticFilePathResovler> files)
        {
            _sharing = sharing;
            _logger = logger;
            _files = files.Value;
        }

        /*
            第一阶段具有以下社交功能:
            1. 发表对音乐的带歌词分享 OK
            3. 回复某个分享 OK
            4. 可以带图分享，支持把图片上传到服务器上 Pending
            5. 点赞某个分享 OK 
            6. (特意不支持删除评论)
         */

        [HttpGet("getcomments")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<JsonResult> GetMusicSharingComments([FromQuery] string musicid)
        {
            return new JsonResult(await _sharing.GetMusicCommentsAsync(musicid));
        }


        [HttpPost("postcomment")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<JsonResult> PostSharingComment([FromQuery] string musicid, [FromBody]SharingComments comment)
        {
            return new JsonResult(await _sharing.CreateMusicSharingComment(musicid, comment));
        }

        [HttpPost("replycomment")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<JsonResult> ReplySharingComment([FromQuery] string musicid, [FromQuery] string commentid, [FromBody] ReplyComments comments)
        {
            return new JsonResult(await _sharing.ReplyMusicSharingComment(musicid, ObjectId.Parse(commentid), comments));
        }

        public void PostSharingPhotos()
        {

        }

        [HttpGet("likecomment")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<JsonResult> LikeComment([FromQuery]string musicid, [FromQuery]string commentid)
        {
            string UserID = User.Claims.ToList()[0].Value;
            return new JsonResult(await _sharing.LikeOneComment(musicid, UserID, ObjectId.Parse(commentid)));
        }
    }
}