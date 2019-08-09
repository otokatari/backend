using OtokatariBackend.Services.Music;
using OtokatariBackend.Services.Token;
using OtokatariBackend.Model.Response;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace OtokatariBackend.Controllers.Music
{
    [Route("music/likesinger")]
    [ApiController]
    [Authorize]
    [ValidateJwtTokenActive]
    public class LikeSingerController : ControllerBase
    {
        private readonly LikeSingerServices _likeSingerServices;
        public LikeSingerController(LikeSingerServices likeSingerServices)
        {
            _likeSingerServices = likeSingerServices;
        }

        [HttpPost("addlike")]
        public async Task<JsonResult> AddLikeSinger([FromQuery] string singerid)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            singerid = singerid.Trim();
            try
            {
                return new JsonResult(await _likeSingerServices.AddSingerLike(ClaimsUserID, ObjectId.Parse(singerid)));
            }
            catch (System.FormatException fmt)
            {
                return new JsonResult(new CommonResponse { StatusCode = -1002 });
            }
        }

        [HttpGet("deletelike")]
        public async Task<JsonResult> DeleteLikeSinger([FromQuery] string singerid)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            singerid = singerid.Trim();
            try
            {
                return new JsonResult(await _likeSingerServices.DeleteSingerLike(ClaimsUserID, ObjectId.Parse(singerid)));
            }
            catch (System.FormatException fmt)
            {
                return new JsonResult(new CommonResponse { StatusCode = -1002 });
            }
        }
    }
}

