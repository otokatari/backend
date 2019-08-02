using System.IO;
using System.Linq;

namespace OtokatariBackend.Utils
{
    public class StaticFilePathResolver
    {
        public string root { get; set; }
        public string avatar { get; set; }
        public string sharing { get; set; }


        public string GetAvatar() => Path.Combine(root, avatar);
        public string GetSharing() => Path.Combine(root, sharing);

        public static string GetFileExtension(string fileName)
        {
            var parts = fileName.Split(".");
            if (parts.Length < 2) return "jpg";
            return parts.Last();
        }
    }
}