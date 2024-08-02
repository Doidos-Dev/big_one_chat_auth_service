using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Auth.UnitTest.Application.Fixture
{
    public static class ClaimsFixture
    {
        
        public static ClaimsPrincipal GenerateClaimsPrincipal(string accessToken, string secret, string issuer)
        {
            byte[] key = Encoding.ASCII.GetBytes(secret);

            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = issuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);
            
            return principal;
        }

        public static JwtSecurityToken GetTokenInformations(string accessToken, string secret, string issuer)
        {
            byte[] key = Encoding.ASCII.GetBytes(secret);

            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = issuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken securityToken)
                return null;

            return securityToken;
        }
    }
}
