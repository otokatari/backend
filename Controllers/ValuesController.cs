using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OtokatariBackend.Persistence.MongoDB.DAO;
using OtokatariBackend.Services.Token;
using XC.RSAUtil;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading;
using OtokatariBackend.Persistence.MongoDB.DAO.SystemLibrary;
using System;
using OtokatariBackend.Persistence.MongoDB.Model;

namespace OtokatariBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly JwtManager jwt;
        private readonly TokenManager tokenManager;
        private readonly RsaPkcs8Util RsaUtils;
        private readonly MongoContext ctx;
        private readonly MusicLibraryOperator _music;

        public ValuesController(IDistributedCache cache,
                                JwtManager jwtMgmr,
                                TokenManager tokenMgmr, 
                                RsaPkcs8Util utils,
                                MongoContext mongoCtx,
                                MusicLibraryOperator music)
        {
            _cache = cache;
            jwt = jwtMgmr;
            tokenManager = tokenMgmr;
            RsaUtils = utils;
            ctx = mongoCtx;
            _music = music;
        }

        [HttpGet("login")]
        [AllowAnonymous]
        public JsonResult Login([FromQuery] string user)
        {
            return new JsonResult(jwt.Create(user));
        }

        [HttpGet("revoke")]
        [Authorize]
        [ValidateJwtTokenActive]
        public async Task<ActionResult<string>> Revoke()
        {
            await tokenManager.RevokeCurrentToken();
            return "Revoke token successfully!";
        }

        [HttpGet("concurrency")]
        public ActionResult<string> Concurrency()
        {
            var t1 = new Thread(() => 
            {
                Console.WriteLine($"1当前 {_music.GetHashCode()}");
                _music.AppendMusicToLibraryIfNotExist(new SimpleMusic
                {
                    Musicid = "Hahaha666",
                    Platform = "netease"
                });
            });
            var t2 = new Thread(() =>
            {
                Console.WriteLine($"2当前 {_music.GetHashCode()}");
                _music.AppendMusicToLibraryIfNotExist(new SimpleMusic
                {
                    Musicid = "Hahaha666",
                    Platform = "netease"
                });
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
            return "OK!!!";
        }
    }
}
