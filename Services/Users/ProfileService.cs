
using OtokatariBackend.Persistence.MySQL.DAO.Users;
using OtokatariBackend.Persistence.MySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OtokatariBackend.Services.Users
{
    public class ProfileService : IOtokatariService
    {
        private readonly UsersDbOperator _users;
        // For profile filter.

        private static readonly IEnumerable<PropertyInfo> ProfileGates = typeof(UserProfilePrivacy).GetProperties().Where(x => x.PropertyType == typeof(byte));
        private static readonly IEnumerable<PropertyInfo> ProfileProperties = typeof(UserProfile).GetProperties();
        public ProfileService(UsersDbOperator user)
        {
            _users = user;
        }
        public UserProfile GetProfile(string QueryUserID, string ClaimsUserID)
        {
            var user = _users.GetProfile(QueryUserID);
            if (user == null) return null;
            else
            {
                return QueryUserID == ClaimsUserID ? user : ApplyPrivacyToUserProfile(user, _users.GetProfilePrivacy(user.Userid));
            }
        }

        private UserProfile ApplyPrivacyToUserProfile(UserProfile profile,UserProfilePrivacy privacy)
        {
            foreach (var item in ProfileProperties)
            {
                var gate = ProfileGates.FirstOrDefault(x => x.Name == item.Name);
                if (gate != null)
                {
                    var gateBit = (byte)gate.GetValue(privacy);
                    if (gateBit == 0)
                    {
                        item.SetValue(profile, GetDefaultValue(item.PropertyType));
                    }
                }
            }
            return profile;
        }

        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
