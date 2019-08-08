using MongoDB.Bson;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Model.Response.Music;
using OtokatariBackend.Persistence.MongoDB.DAO.DbSingers;
using OtokatariBackend.Persistence.MongoDB.DAO.SystemLibrary;
using OtokatariBackend.Persistence.MongoDB.Model;
using OtokatariBackend.Utils;
using System.Linq;
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
            //_singersList.InsertNewSingerInfoToDbIfNotExists(SingerId, Platform, SingerName);

            if (_singersList.GetSingerInfoByPlatformId(SingerId.ToString()) != null)
                return new CommonResponse { StatusCode = 0 };
            if (await _singersList.AddLikeSinger(UserId, SingerId))
                return new CommonResponse { StatusCode = 1 };
            else return new CommonResponse { StatusCode = -3 };
        }

        public async Task<CommonResponse> DeleteSingerLike(string UserId, ObjectId SingerId)
        {
            return new CommonResponse { StatusCode = await _singersList.DeleteLikeSinger(UserId, SingerId) ? 0 : -1 };
        }
    }
}

