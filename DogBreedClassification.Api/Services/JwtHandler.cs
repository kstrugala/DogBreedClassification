using DogBreedClassification.Api.Settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DogBreedClassification.Api.Services
{
    public class JwtHandler : IJwtHandler
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly SecurityKey _securityKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtHeader _jwtHeader;
        private readonly JwtSettings _jwtSettings;


        public JwtHandler(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            _jwtHeader = new JwtHeader(_signingCredentials);
        }

        public string Create(string email, string role)
        {
            var nowUtc = DateTime.UtcNow;
            var expires = nowUtc.AddMinutes(_jwtSettings.ExpiryMinutes);
            var unixStandardTimeBegin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
            var exp = (long)(new TimeSpan(expires.Ticks - unixStandardTimeBegin.Ticks).TotalSeconds);
            var iat = ((long)(new TimeSpan(nowUtc.Ticks - unixStandardTimeBegin.Ticks).TotalSeconds)).ToString();

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.Issuer),
                new Claim(JwtRegisteredClaimNames.Iat, iat, ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Exp, exp.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.UniqueName, email)
            };

            var payload = new JwtPayload(claims);

            var jwt = new JwtSecurityToken(_jwtHeader, payload);
            var token = _jwtSecurityTokenHandler.WriteToken(jwt);

            return token;
        }
    }
}
