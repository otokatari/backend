using System.Collections.Generic;

namespace OtokatariBackend.Model.Response.Sharing
{
    public class UploadPostPhotosResponse : CommonResponse
    {
        public List<string> ReceivedFiles { get; set; }
    }
}