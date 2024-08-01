using Application.Utils.Auth.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Application.Utils.Auth
{
    public class TokensGenerator : ITokensGenerator
    {
        private readonly byte[] _key;
        private readonly IConfiguration _configuration;

        public TokensGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]!);
        }

        public string GenerateToken(UserModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var credentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature);
            var claims = CreateClaims(model);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Issuer = _configuration["JWT:Issuer"],
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddHours(int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_TIME_HOURS")!))
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetClaimsPrincipal(string accessToken)
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

        private ClaimsIdentity CreateClaims(UserModel model)
        {
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, model.Nickname));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, model.Role));

            return claimsIdentity;
        }
    }
}
