using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Model.Response.Sharing;
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
        private readonly StaticFilePathResolver _resolver;
        private readonly SharingServices _sharing;
        private readonly ILogger<SharingController> _logger;
        public SharingController(SharingServices sharing,
                                ILogger<SharingController> logger,
                                IOptions<StaticFilePathResolver> files)
        {
            _sharing = sharing;
            _logger = logger;
            _resolver = files.Value;
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
        public async Task<JsonResult> GetMusicSharingComments([FromQuery] string musicid)
        {
            return new JsonResult(await _sharing.GetMusicSharingComments(musicid));
        }


        [HttpPost("postcomment")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<JsonResult> PostSharingComment([FromQuery] string musicid, [FromBody]SharingComments comment)
        {
            return new JsonResult(await _sharing.PostSharingComment(musicid, comment));
        }

        [HttpPost("replycomment")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<JsonResult> ReplySharingComment([FromQuery] string musicid, [FromQuery] string commentid, [FromBody] ReplyComments comments)
        {
            return new JsonResult(await _sharing.ReplySharingComment(musicid, commentid, comments));
        }

        [HttpGet("getsharingphotos")]
        [Authorize]
        [ValidateJwtTokenActive]
        public IActionResult GetSharingPhotos([FromQuery][Required] string musicid, [FromQuery][Required] string photo)
        {
            var ext = StaticFilePathResolver.GetFileExtension(photo);
            var filePath = Path.Combine(_resolver.GetSharing(),musicid,photo);
            var file = new FileInfo(filePath);
            if(file.Exists)
            {
                return PhysicalFile(file.FullName,$"image/{ext}");
            }
            return NotFound();
        }

        [HttpPost("postphotos")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<CommonResponse> PostSharingPhotos([Required] string musicid, [Required] IList<IFormFile> files)
        {
            if(files.Count == 0) return new CommonResponse { StatusCode = -2 }; // Didn't place files into form-data correctly.
            var sharing = _resolver.GetSharing();
            var folderPath = Path.Combine(sharing, musicid);
            var folderInfo = new DirectoryInfo(folderPath);
            if (!folderInfo.Exists) folderInfo.Create();
            var received = new List<string>();
            var receivedFullPath = new List<string>();
            try
            {
                foreach (var file in files)
                {
                    var fileName = $"{Guid.NewGuid().ToString()}.{StaticFilePathResolver.GetFileExtension(file.FileName)}";
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var fs = System.IO.File.Create(filePath))
                    {
                        await file.OpenReadStream().CopyToAsync(fs);
                    }
                    received.Add(fileName);
                    receivedFullPath.Add(filePath);
                }
                return new UploadPostPhotosResponse { StatusCode = 0, ReceivedFiles = received };
            }
            
            catch (Exception ex)
            {
                _logger.LogError($"Write file error: {ex.Message} {ex.StackTrace}");
                // Regards this upload as failed. Need to free up all space taken up by this failed upload.
                foreach (var filePath in receivedFullPath)
                {
                    var file = new FileInfo(filePath);
                    if(file.Exists) file.Delete();
                }
                return new CommonResponse { StatusCode = -1 }; // Server-side error.
            }
        }

        [HttpGet("likecomment")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<JsonResult> LikeComment([FromQuery] bool like, [FromQuery]string musicid, [FromQuery]string commentid)
        {
            string UserID = User.Claims.ToList()[0].Value;
            if(like) 
            {
                return new JsonResult(await _sharing.LikeComment(musicid,commentid,UserID));
            }
            return new JsonResult(await _sharing.UnLikeComment(musicid,commentid,UserID));
        }
    }
}