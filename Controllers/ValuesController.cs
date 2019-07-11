using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OtokatariBackend.Services.Token;
using XC.RSAUtil;

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

        public ValuesController(IDistributedCache cache,JwtManager jwtMgmr,TokenManager tokenMgmr, RsaPkcs8Util utils)
        {
            _cache = cache;
            jwt = jwtMgmr;
            tokenManager = tokenMgmr;
            RsaUtils = utils;
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
        public ActionResult<string> SecretArea()
        {
            return "You are entered secret area";
        }

        [HttpPost("decrypt")]
        public ActionResult<string> DecrpytRsa([FromForm] string Encrypted)
        {
            return RsaUtils.Decrypt(Encrypted, RSAEncryptionPadding.Pkcs1);
        }
    }
}
