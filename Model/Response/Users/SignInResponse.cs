using OtokatariBackend.Model.DependencyInjection.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Model.Response.Users
{
    public class SignInResponse : CommonResponse
    {
        public string UserID { get; set; }
        public string AccessToken { get; set; }
        public string ExpireTime { get; set; }
    }
}
