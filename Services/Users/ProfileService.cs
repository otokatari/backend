
using OtokatariBackend.Persistence.MySQL.DAO.Users;
using OtokatariBackend.Persistence.MySQL.Model;

namespace OtokatariBackend.Services.Users
{
    public class ProfileService : IOtokatariService
    {
        private readonly UsersDbOperator _users;
        public ProfileService(UsersDbOperator user)
        {
            _users = user;
        }
        public UserProfile GetProfile(string QueryUserID, string ClaimsUserID)
        {
            var user = _users.GetProfile(QueryUserID);
            if (user == null) return null;
            if (QueryUserID == ClaimsUserID)
                return user;
            return new UserProfile { Userid = QueryUserID };
        }
    }
}
