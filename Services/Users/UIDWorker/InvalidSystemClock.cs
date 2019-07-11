using System;

namespace OtokatariBackend.Services.Users.UIDWorker

{
    public class InvalidSystemClock : Exception
    {      
        public InvalidSystemClock(string message) : base(message) { }
    }
}