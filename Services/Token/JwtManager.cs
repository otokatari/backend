using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OtokatariBackend.Model.DependencyInjection.Token;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OtokatariBackend.Services.Token
{
    public class JwtManager
    {
        private readonly JwtTokenConfig _options;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly SecurityKey _securityKey;
        private readonly SigningCredentials _signingCredentials;

        public JwtManager(IOptions<JwtTokenConfig> options)
        {
            _options = options.Value;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
        }

        public JwtResponse Create(string user)
        {
            var expires = DateTime.Now.AddSeconds(_options.ExpireSpan);
            var jwt = new JwtSecurityToken(_options.Issuer, _options.Audience,
                new[]
                {
                    new Claim(ClaimTypes.Name,user)
                },
                expires: expires,
                signingCredentials: _signingCredentials
            );
            var token = _jwtSecurityTokenHandler.WriteToken(jwt);
            return new JwtResponse
            {
                AccessToken = token,
                ExpireTime = expires.ToString("yyyy/M/d HH:mm:ss")
            };
        }

    }
}
