using Microsoft.Extensions.Logging;
using OtokatariBackend.Persistence.MongoDB.DAO.Sharing;
using OtokatariBackend.Persistence.MongoDB.Model;

namespace OtokatariBackend.Services.Music
{
    public class SharingServices : IOtokatariService
    {
        private readonly SharingDbOperator _sharing;
        private readonly ILogger<SharingServices> _logger;

        public SharingServices(SharingDbOperator sharing,ILogger<SharingServices> logger)
        {
            _sharing = sharing;
            _logger = logger;
        }

        // public MusicComments GetMusicSharingComments()
        // {
            
        // }
    }
}