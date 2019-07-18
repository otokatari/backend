using System.IO;

namespace OtokatariBackend.Utils
{
    public class StaticFilePathResovler
    {
        public string root { get; set; }
        public string avatar { get; set; }
        public string sharing { get; set; }


        public string GetAvatar() => Path.Combine(root,avatar);
        public string GetSharing() => Path.Combine(root,sharing);
    }
}