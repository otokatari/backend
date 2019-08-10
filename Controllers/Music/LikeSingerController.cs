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

        [HttpGet("addlike")]
        public async Task<JsonResult> AddLikeSinger([FromQuery] string singerid)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            singerid = singerid.Trim();

            return new JsonResult(await _likeSingerServices.AddSingerLike(ClaimsUserID, ObjectId.Parse(singerid)));


        }

        [HttpGet("deletelike")]
        public async Task<JsonResult> DeleteLikeSinger([FromQuery] string singerid)
        {
            string ClaimsUserID = User.Claims.FirstOrDefault()?.Value;
            singerid = singerid.Trim();
            return new JsonResult(await _likeSingerServices.DeleteSingerLike(ClaimsUserID, ObjectId.Parse(singerid)));
        }
    }
}

