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
        public UsersDbOperator(OtokatariContext context, ILogger<UsersDbOperator> loggger,IdWorker uid)
        {
            _context = context;
            _logger = loggger;
            _uid = uid;
        }

        public UserLogin SignIn(string Identifier, string Credentials, int Type)
            =>    _context.UserLogin.FirstOrDefault
                  (x => x.Identifier == Identifier &&
                   x.Credentials == Credentials &&
                   x.Type == Type);

        public async Task<UserLogin> SignUp(string Identifier,string Credentials,int Type)
        {
            var ExistsUser = _context.UserLogin.FirstOrDefault(x => x.Identifier == Identifier && x.Type == Type);
            if (ExistsUser != null) return new UserLogin();

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var NewUser = new UserLogin { Credentials = Credentials, Identifier = Identifier, Type = (byte)Type, Userid = _uid.NextId().ToString() };
                _context.UserLogin.Add(NewUser);
                int affects = await _context.SaveChangesAsync();
                if (affects == 1)
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
    }
}
