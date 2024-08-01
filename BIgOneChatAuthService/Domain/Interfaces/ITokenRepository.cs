using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITokenRepository
    {
        Task<TokenModel> Register(TokenModel model);
        Task<TokenModel> GetByNickname(string nickname);
        Task<TokenModel> Update(TokenModel model);
    }
}
