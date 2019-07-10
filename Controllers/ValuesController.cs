using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OtokatariBackend.Services.Token;

namespace OtokatariBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly JwtManager jwt;
        private readonly TokenManager tokenManager;

        public ValuesController(IDistributedCache cache,JwtManager jwtMgmr,TokenManager tokenMgmr)
        {
            _cache = cache;
            jwt = jwtMgmr;
            tokenManager = tokenMgmr;
        }

        [HttpGet("login")]
        [AllowAnonymous]
        public JsonResult Login([FromQuery] string user)
        {
           // HttpContext.Authentication.SignInAsync()
            
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
    }
}
