using Domain.Models;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenModel> GenerateToken(UserModel model);
        Task<TokenModel> RefreshToken(UserModel model);
    }
}
