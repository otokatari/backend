using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtokatariBackend.Model.Request.Users;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Services.Token;
using OtokatariBackend.Services.Users;

namespace OtokatariBackend.Controllers.Users
{
    [Route("user/auth")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityService _identity;
        private readonly TokenManager _token;
        public IdentityController(IdentityService identity,TokenManager token)
        {
            _identity = identity;
            _token = token;

        }

        [HttpPost("login")]
        public JsonResult SignIn([FromBody] SignInRequest signin)
        {
            return new JsonResult(_identity.SignIn(signin));
        }

        [HttpPost("register")]
        public async Task<JsonResult> SignUp([FromBody] SignInRequest signup)
        {
            return new JsonResult(await _identity.SignUp(signup));
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<JsonResult> Signout()
        {
            await _token.RevokeCurrentToken();
            return new JsonResult(new CommonResponse { StatusCode = 0 });
        }
    }
}