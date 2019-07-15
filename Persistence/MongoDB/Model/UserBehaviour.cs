using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OtokatariBackend.Persistence.MongoDB.Model
{
    public class UserBehaviour
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("userid")]
        public string Userid { get; set; }

        [BsonElement("behaviour")]
        public string Behaviour { get; set; }

        [BsonElement("time")]
        public int Time { get; set; }

        [BsonElement("music")]
        public SimpleMusic Music { get; set; }

        [BsonElement("isinplaylist")]
        public string Isinplaylist { get; set; }
    }
}
