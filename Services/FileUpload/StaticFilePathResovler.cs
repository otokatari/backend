using System.IO;
using Microsoft.Extensions.Logging;

namespace OtokatariBackend.Utils
{
    public class StaticFilePathResovler
    {
        public string root { get; set; }
        public string avatar { get; set; }
        public string sharing { get; set; }


        public string GetAvatar() => Path.Combine(root, avatar);
        public string GetSharing() => Path.Combine(root, sharing);
        public StaticFilePathResovler()
        {
            var dirs = new[] { root, GetAvatar(), GetSharing() };
            foreach (var dir in dirs)
            {
                var DirInfo = new DirectoryInfo(dir);
                if(!DirInfo.Exists)
                    DirInfo.Create();
                System.Console.WriteLine($"Static file folder {dir} loaded");
            }
        }
    }
}