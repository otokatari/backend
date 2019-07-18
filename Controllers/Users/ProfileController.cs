using System;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Persistence.MySQL.Model;
using OtokatariBackend.Services.FileUpload;
using OtokatariBackend.Services.Token;
using OtokatariBackend.Services.Users;
using OtokatariBackend.Utils;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace OtokatariBackend.Controllers.Users
{
    [Route("user/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profile;
        private readonly StaticFilePathResovler _resolver;

        public ProfileController(ProfileService profile, IOptions<StaticFilePathResovler> resolver)
        {
            _profile = profile;
            _resolver = resolver.Value;
        }

        [HttpGet("getprofile")]
        [Authorize]
        [ValidateJwtTokenActive]
        public JsonResult Getprofile([FromQuery] string userid)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            return new JsonResult(_profile.GetProfile(userid, ClaimsUserID));
        }

        [HttpPost("setprofile")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<JsonResult> ModifyProfile([FromQuery] string userid, [FromBody] UserProfile profile)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            if (userid != ClaimsUserID) return new JsonResult(new CommonResponse { StatusCode = -1 });
            return new JsonResult(await _profile.ModifyProfile(userid, profile));
        }

        [HttpGet("getavatar")]
        [Authorize]
        [ValidateJwtTokenActive]
        public IActionResult GetAvatar([FromQuery][Required] string avatar)
        {
            var path = Path.Combine(_resolver.GetAvatar(),avatar);
            var mime = $"image/{avatar.Split(".")[1]}";
            return PhysicalFile(path,mime);
        }
        

        [HttpPost("changeavatar")]
        [Authorize]
        [ValidateJwtTokenActive]
        [DisableFormValueModelBinding]
        [RequestFormLimits(MultipartBodyLengthLimit = 2 * 1024 * 1000)]
        public async Task<JsonResult> UpdateAvatar()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(HttpContext.Request.ContentType))
            {
                return new JsonResult(new CommonResponse { StatusCode = -1 }); // 没有在Header处声明Multipart/form-data.
            }
            var UserID = User.Claims.ToList()[0].Value;
            try
            {

                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), 70);

                var reader = new MultipartReader(boundary, Request.Body);
                var section = await reader.ReadNextSectionAsync();
                string fileExt = string.Empty;
                while (section != null)
                {
                    ContentDispositionHeaderValue contentDisposition;
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {

                            var fileName = contentDisposition.FileName.HasValue ? contentDisposition.FileName.Value.Trim() : string.Empty;
                            if (string.IsNullOrEmpty(fileName))
                            {
                                return new JsonResult(new CommonResponse() { StatusCode = -5 }); // 没有fileName
                            }

                            var fileNamez = fileName.Split(".");
                            if (fileNamez.Length < 2) return new JsonResult(new CommonResponse() { StatusCode = -6 }); // fileName没有后缀

                            fileExt = fileNamez[1];

                            var path = Path.Combine(_resolver.GetAvatar(), $"{UserID}.{fileExt}");
                            using (var targetStream = System.IO.File.Create(path))
                            {
                                await section.Body.CopyToAsync(targetStream);
                            }
                        }
                    }

                    section = await reader.ReadNextSectionAsync();
                }

                var CurrentUserProfile = _profile.GetProfile(UserID, UserID);
                CurrentUserProfile.Avatar = $"{UserID}.{fileExt}";
                
                return new JsonResult(await _profile.ModifyProfile(UserID,CurrentUserProfile));
            }
            catch (InvalidDataException exceed)
            {
                System.Console.WriteLine(exceed.Message);
                return new JsonResult(new CommonResponse { StatusCode = -2 }); // 上传的图片大小超过了2MB
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); // 服务器的其他未知错误。
                return new JsonResult(new CommonResponse { StatusCode = -3 }); // 服务器发生未知错误
            }

        }
    }
}