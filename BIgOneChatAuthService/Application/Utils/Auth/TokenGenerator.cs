using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Application.Utils.Auth
{
    public static class TokenGenerator
    {
        private static readonly byte[] _key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);

        public static string GenerateToken(UserModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var credentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature);
            var claims = CreateClaims(model);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_TIME_HOURS")!))
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal? GetClaimsPrincipal(string accessToken)
        {
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                      !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                     StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }

        private static ClaimsIdentity CreateClaims(UserModel model)
        {
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, model.Nickname));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, model.Role));

            return claimsIdentity;
        }
    }
}
