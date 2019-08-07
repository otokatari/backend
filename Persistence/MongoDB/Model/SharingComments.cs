using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OtokatariBackend.Persistence.MongoDB.Model
{
    public class SharingComments
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("time")]
        public int Time { get; set; }

        [BsonElement("like")]
        public string[] Like { get; set; }

        [BsonElement("userid")]
        public string Userid { get; set; }

        [BsonElement("photo")]
        public string[] Photo { get; set; }

        [BsonElement("comments")]
        public ReplyComments[] Comments { get; set; }

        [BsonElement("sharinglyric")]
        public string[] SharingLyric { get; set; }
    }
}
