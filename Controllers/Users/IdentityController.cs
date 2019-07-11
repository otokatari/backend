using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OtokatariBackend.Model.Request.Users;
using OtokatariBackend.Services.Users;

namespace OtokatariBackend.Controllers.Users
{
    [Route("user/auth")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityService _identity;
        public IdentityController(IdentityService identity)
        {
            _identity = identity;

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
    }
}