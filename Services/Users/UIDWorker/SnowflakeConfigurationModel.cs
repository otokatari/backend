using System;
using System.Collections.Generic;
using System.Text;

namespace OtokatariBackend.Services.Users.UIDWorker
{
    public class SnowflakeConfigurationModel
    {
        public int WorkerId { get; set; }
        public int DatacenterId { get; set; }
    }
}
