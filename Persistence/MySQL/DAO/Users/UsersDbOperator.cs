using Microsoft.Extensions.Logging;
using OtokatariBackend.Persistence.MySQL.Model;
using OtokatariBackend.Services;
using OtokatariBackend.Services.Users.UIDWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Persistence.MySQL.DAO.Users
{
    public class UsersDbOperator : IOtokatariDbOperator
    {
        public enum SignupStatus
        {
            FINISH,
            USER_EXISTS,
            DB_CONCURRENCY
        }

        private readonly OtokatariContext _context;
        private readonly ILogger<UsersDbOperator> _logger;
        private readonly IdWorker _uid;
        public UsersDbOperator(OtokatariContext context, ILogger<UsersDbOperator> loggger, IdWorker uid)
        {
            _context = context;
            _logger = loggger;
            _uid = uid;
        }

        public UserLogin SignIn(string Identifier, string Credentials, int Type)
            => _context.UserLogin.FirstOrDefault
                  (x => x.Identifier == Identifier &&
                   x.Credentials == Credentials &&
                   x.Type == Type);

        public async Task<UserLogin> SignUp(string Identifier, string Credentials, int Type)
        {
            var ExistsUser = _context.UserLogin.FirstOrDefault(x => x.Identifier == Identifier && x.Type == Type);
            if (ExistsUser != null) return new UserLogin();

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userid = _uid.NextId().ToString();
                // Add a new user login info.
                var NewUser = new UserLogin { Credentials = Credentials, Identifier = Identifier, Type = (byte)Type, Userid = userid };
                _context.UserLogin.Add(NewUser);

                // Add a new blank user profile.
                var NewUserProfile = new UserProfile { Userid = userid, Nickname = $"新用户{userid.Substring(8)}" };
                _context.UserProfile.Add(NewUserProfile);

                // Add a new user profile privacy configuration. default all profile fields are public.
                var NewUserProfilePrivacyConfig = new UserProfilePrivacy { Userid = userid };
                _context.UserProfilePrivacy.Add(NewUserProfilePrivacyConfig);

                int affects = await _context.SaveChangesAsync();
                if (affects == 3)
                {
                    transaction.Commit();
                    return NewUser;
                }
                _logger.LogError($"The number of affected records is wrong: {affects}. Rollback.");
                transaction.Rollback();
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"Database update mistake! {e.Message}");
                transaction.Rollback();
            }
            return null;
        }

        public UserProfile GetProfile(string id)
            => _context.UserProfile.FirstOrDefault
                (x => x.Userid == id);

        public IQueryable<UserProfile> GetProfiles(IEnumerable<string> userids)
        {
            return _context.UserProfile.Where(x => userids.Contains(x.Userid));
        }
        public IQueryable<UserProfilePrivacy> GetProfilePrivacies(IEnumerable<string> userids)
        {
            return _context.UserProfilePrivacy.Where(x => userids.Contains(x.Userid));
        }
        public UserProfilePrivacy GetProfilePrivacy(string id) => _context.UserProfilePrivacy.FirstOrDefault(x => x.Userid == id);


        public async Task<bool> UpdateUserProfile(UserProfile profile)
        {
            var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.UserProfile.Update(profile);
                if (1 == await _context.SaveChangesAsync())
                {
                    trans.Commit();
                    return true;
                }
                trans.Rollback();
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Database update mistake! {e.Message}");
                trans.Rollback();
            }
            return false;
        }


        public async Task<bool> UpdateProfilePrivacy(UserProfilePrivacy privacy)
        {
            var tr = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.UserProfilePrivacy.Update(privacy);
                if (1 == await _context.SaveChangesAsync())
                {
                    tr.Commit();
                    return true;
                }
                tr.Rollback();
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Database update mistake! {e.Message}");
                tr.Rollback();
            }
            return false;
        }
    }
}
