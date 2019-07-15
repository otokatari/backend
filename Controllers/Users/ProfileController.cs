using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Persistence.MySQL.Model;
using OtokatariBackend.Services.Token;
using OtokatariBackend.Services.Users;

namespace OtokatariBackend.Controllers.Users
{
    [Route("user/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profile;
        public ProfileController(ProfileService profile)
        {
            _profile = profile;
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
    }
}