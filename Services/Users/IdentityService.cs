using Microsoft.Extensions.Logging;
using OtokatariBackend.Model.Request.Users;
using OtokatariBackend.Model.Response;
using OtokatariBackend.Model.Response.Users;
using OtokatariBackend.Persistence.MySQL.DAO.Users;
using OtokatariBackend.Services.Token;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using XC.RSAUtil;

namespace OtokatariBackend.Services.Users
{
    public class IdentityService : IOtokatariService
    {
        private readonly UsersDbOperator _users;
        private readonly JwtManager _jwt;
        private readonly RsaPkcs8Util _rsa;

        private readonly ILogger<IdentityService> _logger;
        public IdentityService(UsersDbOperator users,JwtManager jwt,RsaPkcs8Util rsa,ILogger<IdentityService> logger)
        {
            _users = users;
            _jwt = jwt;
            _rsa = rsa;
            _logger = logger;
        }

        public SignInResponse SignIn(SignInRequest signin)
        {
            try
            {
                var DecryptedCredentials = DecryptCredentials(signin);

                var user = _users.SignIn(signin.identifier, DecryptedCredentials, signin.type);
                if (user == null) return new SignInResponse { StatusCode = -1 };
                var token = _jwt.Create(user.Userid);
                return new SignInResponse
                {
                    StatusCode = 0,
                    AccessToken = token.AccessToken,
                    ExpireTime = token.ExpireTime,
                    UserID = user.Userid
                };
            }
            catch (CryptographicException crypto)
            {
                // The length of the data to decrypt is not valid for the size of this key.
                _logger.LogError($"Decrypt RSA-Encrpted password failed. Reason: {crypto.Message}");
                return new SignInResponse { StatusCode = -1000 };
            }
            catch(FormatException fmt)
            {
                _logger.LogError($"Meeting an invalid base64 string. Reason: {fmt.Message}");
                return new SignInResponse { StatusCode = -1001 };
            }
        }

        public async Task<CommonResponse> SignUp(SignInRequest signup)
        {
            try
            {
                var DecryptedCredentials = DecryptCredentials(signup); // first extract credentials from encapsulted object
                
                var user = await _users.SignUp(signup.identifier, DecryptedCredentials, signup.type);
                if (user == null) return new CommonResponse { StatusCode = -2 };
                if (string.IsNullOrEmpty(user?.Userid)) return new CommonResponse { StatusCode = -1 };
                return new CommonResponse { StatusCode = 0 };

            }

            catch (CryptographicException crypto)
            {
                // The length of the data to decrypt is not valid for the size of this key.
                _logger.LogError($"Decrypt RSA-Encrpted password failed. Reason: {crypto.Message}");
                return new CommonResponse { StatusCode = -1000 };
            }
            catch (FormatException fmt)
            {
                _logger.LogError($"Meeting an invalid base64 string. Reason: {fmt.Message}");
                return new CommonResponse { StatusCode = -1001 };
            }
        }

        private string DecryptCredentials(SignInRequest sign) => _rsa.Decrypt(sign.credentials, RSAEncryptionPadding.Pkcs1);
    }
}
