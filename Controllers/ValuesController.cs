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

        public ValuesController(IDistributedCache cache,
                                JwtManager jwtMgmr,
                                TokenManager tokenMgmr, 
                                RsaPkcs8Util utils,
                                MongoContext mongoCtx)
        {
            _cache = cache;
            jwt = jwtMgmr;
            tokenManager = tokenMgmr;
            RsaUtils = utils;
            ctx = mongoCtx;
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

        [HttpGet("secret")]
        [Authorize]
        [ValidateJwtTokenActive]
        public JsonResult SecretArea([FromQuery] string musicid)
        {   
            return new JsonResult(ctx.MusicComments.AsQueryable().FirstOrDefault(x => x.Musicid == musicid));   
        }

        [HttpPost("decrypt")]
        public ActionResult<string> DecrpytRsa([FromForm] string Encrypted)
        {
            return RsaUtils.Decrypt(Encrypted, RSAEncryptionPadding.Pkcs1);
        }
    }
}
