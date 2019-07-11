using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Services.Token
{
    public class ValidateJwtTokenActiveAttribute : TypeFilterAttribute
    {
        public ValidateJwtTokenActiveAttribute() : base(typeof(ValidateJwtToken))
        {

        }
    }

    public class ValidateJwtToken : IAsyncAuthorizationFilter
    {
        private readonly TokenManager token;
        public ValidateJwtToken(TokenManager tokenMgmr)
        {
            token = tokenMgmr;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (await token.IsCurrentTokenActive())
            {
                return;
            }
            context.Result = new UnauthorizedResult();
        }

    }
}
