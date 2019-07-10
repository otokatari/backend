using System;
using System.Collections.Generic;

namespace OtokatariBackend.Persistence
{
    public partial class UserProfile
    {
        public long Userid { get; set; }
        public byte Sex { get; set; }
        public string Nickname { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Avatar { get; set; }
        public string Signature { get; set; }
        public long? Birthday { get; set; }
    }
}
