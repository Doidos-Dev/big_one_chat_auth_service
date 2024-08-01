using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils.Auth.Interfaces
{
    public interface ITokensGenerator
    {
        string GenerateToken(UserModel model);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetClaimsPrincipal(string accessToken);
    }
}
