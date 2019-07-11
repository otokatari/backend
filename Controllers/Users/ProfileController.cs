using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public JsonResult Getprofile([FromBody] string givenid)
        {
            string userid = User.Claims.ToList()[0].ToString();
            return new JsonResult(_profile.Getprofile(givenid, userid));
        }
    }
}