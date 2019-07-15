using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OtokatariBackend.Persistence.MongoDB.Model
{
    public class MusicComments
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("musicid")]
        public string Musicid { get; set; }

        [BsonElement("comments")]
        public SharingComments[] Comments { get; set; }

    }
}
