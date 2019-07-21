using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Services;

namespace OtokatariBackend.Persistence.MongoDB.DAO.Sharing
{
    public class SharingDbOperator : IOtokatariDbOperator
    {
        private readonly MongoContext _context;
        private readonly ILogger<SharingDbOperator> _logger;
        public SharingDbOperator(MongoContext context,
                                ILogger<SharingDbOperator> logger)
        {
            _context = context;
            _logger = logger;
        }

        public  async Task<MusicComments> GetMusicCommentsAsync(string Musicid)
        {
            var MusicCommentRootExistsFilter = Builders<MusicComments>.Filter.Eq("musicid", Musicid);
            return await (await _context.MusicComments.FindAsync(MusicCommentRootExistsFilter)).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateMusicSharingComment(string Musicid, SharingComments comments)
        {
            var MusicCommentRootExistsFilter = Builders<MusicComments>.Filter.Eq(r => r.Musicid, Musicid);
            var finder = await _context.MusicComments.FindAsync(MusicCommentRootExistsFilter);
            var music = await finder.FirstOrDefaultAsync();
            if (music != null)
            {
                var inserter = Builders<MusicComments>.Update.Push("comments", comments);
                var result = await _context.MusicComments.UpdateOneAsync(MusicCommentRootExistsFilter, inserter);
                return comments._id != null;
            }
            else
            {
                // Need to create a new MusicComments record.

                var MusicCommentRoot = new MusicComments
                {
                    Musicid = Musicid,
                    Comments = new [] { comments }
                };

                await _context.MusicComments.InsertOneAsync(MusicCommentRoot);
                return MusicCommentRoot._id != null;
            }
        }


        public async Task<bool> ReplyMusicSharingComment(string Musicid,ObjectId Commentid,ReplyComments comments)
        {
            // var musicFilter = Builders<MusicComments>.Filter.Eq("musicid",Musicid);
            // var root = await (await _context.MusicComments.FindAsync(musicFilter)).FirstOrDefaultAsync();;
            // if(root != null)
            // {
            //     Builders<MusicComments>.Update.Push("comments.$[].comments",comments);
            // }
            // return false;
            comments._id = ObjectId.GenerateNewId();
            var filters = Builders<MusicComments>.Filter.Eq(r => r.Musicid,Musicid) & Builders<MusicComments>.Filter.Eq("comments._id",Commentid);

            var pusher = Builders<MusicComments>.Update.Push("comments.$[].comments",comments);

            var updated = await _context.MusicComments.UpdateOneAsync(filters,pusher);
            return updated.ModifiedCount == 1;
        }


        public async Task<bool> LikeOneComment(string Musicid,string LikeUserid,ObjectId Commentid)
        {
            var filter = Builders<MusicComments>.Filter;
            var filters = filter.Eq(r => r.Musicid,Musicid) & filter.Eq("comments._id",Commentid) & !filter.AnyEq("comments.like",LikeUserid);

            var pusher = Builders<MusicComments>.Update.Push("comments.$[].like",LikeUserid);

            var updated = await _context.MusicComments.UpdateOneAsync(filters,pusher);
            return updated.ModifiedCount == 1;
        }

        public async Task<bool> UnLikeOneComment(string Musicid,string LikeUserid,ObjectId Commentid)
        {
            var filter = Builders<MusicComments>.Filter;
            var filters = filter.Eq(r => r.Musicid,Musicid) & filter.Eq("comments._id",Commentid) & filter.AnyEq("comments.like",LikeUserid);;

            var pusher = Builders<MusicComments>.Update.Pull("comments.$[].like",LikeUserid);

            var updated = await _context.MusicComments.UpdateOneAsync(filters,pusher);
            return updated.ModifiedCount == 1;
        }
    }
}