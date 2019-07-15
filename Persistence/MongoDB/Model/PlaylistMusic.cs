using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using OtokatariBackend.Utils.TypeMerger;

namespace OtokatariBackend.Persistence.MongoDB.Model
{
    public class PlaylistMusic : SimpleMusic
    {
        [BsonElement("addedtime")]
        public int AddedTime { get; set; }


    }

}
