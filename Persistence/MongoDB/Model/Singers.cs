using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OtokatariBackend.Persistence.MongoDB.Model
{
    public class Singers
    {

        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        [BsonElement("SingerName")]
        public string SingerName { get; set; }
        [BsonElement("Language")]
        public string[] Language { get; set; }
        [BsonElement("NeteaseId")]
        public string NeteaseId { get; set; }
        [BsonElement("KugouId")]
        public string KugouId { get; set; }
        [BsonElement("QQMusicId")]
        public string QQMusicId { get; set; }
    }
}