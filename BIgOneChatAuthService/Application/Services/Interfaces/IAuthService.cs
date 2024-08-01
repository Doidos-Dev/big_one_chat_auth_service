using Domain.Models;
using Application.Responses;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<APIResponse<TokenModel>> GenerateToken(UserModel model);
        Task<APIResponse<TokenModel>> RefreshToken(TokenModel model);
    }
}
