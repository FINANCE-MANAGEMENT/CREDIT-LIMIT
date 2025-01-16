using DealerNetAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DealerNetAPI.Helper
{
    public static class JwtHelpers
    {
        //Here GetClaims() Method is used to create return claims list from user token details.
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, Guid Id, Int32 tokenExpiredMinutes, string userName)
        {
            IEnumerable<Claim> claims = new Claim[] {
                new Claim("Id", userAccounts.UserId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userAccounts.EmailId),
                new Claim(ClaimTypes.Role, userAccounts.Role.RoleId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddMinutes(tokenExpiredMinutes).ToString("MMM ddd dd yyyy HH:mm:ss tt")) // Expiration Time set here
            };
            return claims;
        }
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, out Guid Id, Int32 tokenExpiredMinutes, string userName)
        {
            Id = Guid.NewGuid();
            return GetClaims(userAccounts, Id, tokenExpiredMinutes, userName);
        }
        public static UserTokens GenTokenkey(UserTokens model, JwtSettings jwtSettings, IConfiguration configuration, string userName)
        {
            try
            {
                int tokenExpiredMinutes = Convert.ToInt32(configuration["TokenExpiredMinutes"]);
                var UserToken = new UserTokens();
                if (model == null) throw new ArgumentException(nameof(model));
                // Get secret key
                var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);
                Guid Id = Guid.Empty;
                DateTime expireTime = DateTime.Now.AddMinutes(tokenExpiredMinutes); // Expiration Time set here
                UserToken.Validaty = expireTime.TimeOfDay;
                UserToken.ExpiredTime = expireTime;
                var JWToken = new JwtSecurityToken(issuer: jwtSettings.ValidIssuer,
                                                   audience: jwtSettings.ValidAudience,
                                                   claims: GetClaims(model, out Id, tokenExpiredMinutes, userName),
                                                   notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                                                   expires: new DateTimeOffset(expireTime).DateTime,
                                                   signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
                UserToken.Token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                UserToken.EmailId = model.EmailId;
                UserToken.MobileNo = model.MobileNo;
                UserToken.UserId = model.UserId;
                UserToken.GuidId = Id;
                return UserToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static ClaimsPrincipal GetPrincipleFromExpiredToken(JwtSettings jwtSettings, string token)
        {
            // Get secret key
            var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is Invalid Token");
            return principal;

        }


    }
}