using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OtokatariBackend.Persistence.MongoDB.DAO;
using OtokatariBackend.Services.Token;
using XC.RSAUtil;
using System.Threading;
using OtokatariBackend.Persistence.MongoDB.DAO.SystemLibrary;
using System;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services.FileUpload;
using OtokatariBackend.Utils;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Net.Http;

namespace OtokatariBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly JwtManager jwt;
        private readonly TokenManager tokenManager;
        private readonly RsaPkcs8Util RsaUtils;
        private readonly MongoContext ctx;
        private readonly MusicLibraryOperator _music;

        public ValuesController(IDistributedCache cache)
        {

        }

        [HttpPost("tryupload")]
        [DisableFormValueModelBinding]
        public async Task<ActionResult<string>> TryUpload()
        {
            // Used to accumulate all the form url encoded key value pairs in the 
            // request.

            string targetFilePath = "/Users/cupdesk-mbp/CodeFiles/otokatari/avatar/relax.jpg";

            var file = Request.Body;

            if (file.CanRead)
            {
                //  string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim().ToString();
                string fullPath = targetFilePath;
                using (var stream = new FileStream(fullPath,FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return "OK!";
        }
    }
}
