using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}