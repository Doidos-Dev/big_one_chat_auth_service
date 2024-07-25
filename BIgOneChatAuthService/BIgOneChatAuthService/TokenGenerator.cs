using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BIgOneChatAuthService
{
    public static class TokenGenerator
    {

        public static string GenerateToken(UserModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var claims = CreateClaims(model);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private static ClaimsIdentity CreateClaims(UserModel model)
        {
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, model.Name));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, model.Role));

            return claimsIdentity;
        }
    }
}
