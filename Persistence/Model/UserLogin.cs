using Newtonsoft.Json;

namespace OtokatariBackend.Persistence
{
    public partial class UserLogin
    {
        public long Userid { get; set; }
        public byte Type { get; set; }
        public string Identifier { get; set; }
        [JsonIgnore]
        public string Credentials { get; set; }
    }
}
