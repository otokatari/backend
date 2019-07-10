using System;
using System.Collections.Generic;

namespace OtokatariBackend.Persistence
{
    public partial class Follows
    {
        public long Userid { get; set; }
        public long FollowUserid { get; set; }
    }
}
