using Application.Services.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        public Task<TokenModel> GenerateToken(UserModel model)
        {
            throw new NotImplementedException();
        }

        public Task<TokenModel> RefreshToken(UserModel model)
        {
            throw new NotImplementedException();
        }
    }
}
