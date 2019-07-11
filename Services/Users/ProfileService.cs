using OtokatariBackend.Persistence.MySQL.DAO.Users;
using OtokatariBackend.Model.Response.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Services.Users
{
    public class ProfileService : IOtokatariService
    {
        private readonly UsersDbOperator _users;
        public GetprofileResponse Getprofile(string givenid, string userid)
        {
            var user = _users.GetProfile(givenid);
            if (user == null) return new GetprofileResponse { StatusCode = -1 };
            if (givenid == userid)
                return new GetprofileResponse
                {
                    StatusCode = 0,
                    Sex = user.Sex,
                    Nickname = user.Nickname,
                    Country = user.Country,
                    City = user.City,
                    Avatar = user.Avatar,
                    Signature = user.Signature,
                    Birthday = user.Birthday
                };
            else return new GetprofileResponse
            {
                StatusCode = 0
            };
        }
    }
}
