using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services.Music;
using OtokatariBackend.Services.Token;

namespace OtokatariBackend.Controllers.Music
{
    [Route("music/tracking")]
    [ApiController]
    [Authorize]
    [ValidateJwtTokenActive]
    public class MusicLibraryController : ControllerBase
    {
        private readonly BehaviourServices _context;
        private readonly ILogger<MusicLibraryController> _logger;

        public MusicLibraryController(BehaviourServices context, ILogger<MusicLibraryController> logger)
        {
            _logger = logger;
            _context = context;
        }


        [HttpPost("behaviour")]
        public async Task<JsonResult> ReportBehavior(UserBehaviour behaviour) => new JsonResult(await _context.ReportBehavior(behaviour));

    }
}