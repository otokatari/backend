using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Model.Response.Users
{
    public class GetprofileResponse : CommonResponse
    {
        public byte Sex { get; set; }
        public string Nickname { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Avatar { get; set; }
        public string Signature { get; set; }
        public long? Birthday { get; set; }
    }
}
