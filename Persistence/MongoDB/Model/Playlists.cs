using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OtokatariBackend.Persistence.MongoDB.Model
{
    public class Playlists
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("userid")]
        public string Userid { get; set; }

        [BsonElement("favourite")]
        public bool Favourite { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("songs")]
        public PlaylistMusic[] Songs { get; set; }

        [BsonElement("createTime")]
        public int CreateTime { get; set; }
    }
}
