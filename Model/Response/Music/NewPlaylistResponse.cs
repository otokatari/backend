using MongoDB.Bson;

namespace OtokatariBackend.Model.Response.Music
{
    public class NewPlaylistResponse : CommonResponse
    {
        public ObjectId PlaylistId { get; set; }

    }
}
