﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Persistence.MongoDB.DependencyInjection
{
    public class MongoClientConfiguration
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }

    }
}
