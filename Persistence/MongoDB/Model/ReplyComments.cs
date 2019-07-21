using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OtokatariBackend.Persistence.MongoDB.Model
{
    public class ReplyComments
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("time")]
        public int Time { get; set; }

        [BsonElement("userid")]
        public string Userid { get; set; }

        [BsonElement("replyto")]
        public string ReplyTo { get; set; }
    }
}
