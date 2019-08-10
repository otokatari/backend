using System;
using System.Collections.Generic;

namespace OtokatariBackend.Middlewares
{
    public class ErrorHandlerOptions
    {
        public Dictionary<Type,int> ErrorCodes { get; set; }
    }
}