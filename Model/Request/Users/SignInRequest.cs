using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Model.Request.Users
{
    public class SignInRequest
    {
        public string identifier { get; set; }
        public string credentials { get; set; }
        public int type { get; set; }

    }
}
