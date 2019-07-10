using System;
using System.Collections.Generic;

namespace OtokatariBackend.Persistence
{
    public partial class UserLogin
    {
        public long Userid { get; set; }
        public byte Type { get; set; }
        public string Identifier { get; set; }
        public string Credentials { get; set; }
    }
}
