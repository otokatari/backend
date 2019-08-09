using MongoDB.Bson;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Persistence.MongoDB.DAO.DbSingers;
using System.Threading.Tasks;

namespace OtokatariBackend.Services.Music
{
    public class LikeSingerServices : IOtokatariService
    {
        private readonly SingersDbOperator _singersList;
        public LikeSingerServices(SingersDbOperator singersList)
        {
            _singersList = singersList;
        }

        public async Task<CommonResponse> AddSingerLike(string UserId, ObjectId SingerId)
        {
            if (await _singersList.AddLikeSinger(UserId, SingerId))
                return new CommonResponse { StatusCode = 0 };
            return new CommonResponse { StatusCode = -1 };
        }

        public async Task<CommonResponse> DeleteSingerLike(string UserId, ObjectId SingerId)
        {
            return new CommonResponse { StatusCode = await _singersList.DeleteLikeSinger(UserId, SingerId) ? 0 : -1 };
        }
    }
}

