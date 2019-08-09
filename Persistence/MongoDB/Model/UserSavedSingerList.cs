using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OtokatariBackend.Persistence.MongoDB.Model
{
    public class UserSavedSingerList<TList>
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        [BsonElement("Userid")]
        public string Userid { get; set; }
        [BsonElement("SavedList")]
        public TList[] SavedList { get; set; }
        [BsonElement("SystemList")]
        public TList[] SystemList { get; set; }
    }
}