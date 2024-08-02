using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.UnitTest.Application.Fixture
{
    public static class JWTTokenFixture
    {
        public static string GenerateToken(UserModel model, string secret, string issuer, int expTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var claims = CreateClaims(model);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Issuer = issuer,
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddHours(expTime)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static ClaimsIdentity CreateClaims(UserModel model)
        {
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, model.Nickname));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, model.Role));

            return claimsIdentity;
        }
    }
}
