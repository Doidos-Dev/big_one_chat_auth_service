using Application.Responses;
using Application.Services.Interfaces;
using Application.Utils.Auth;
using Application.Utils.Auth.Interfaces;
using Domain.Interfaces;
using Domain.Models;


namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly ITokensGenerator _tokenGenerator;

        public AuthService(ITokenRepository tokenRepository, ITokensGenerator tokenGenerator)
        {
            _tokenRepository = tokenRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<APIResponse<TokenModel>> GenerateToken(UserModel model)
        {
            var accessToken = _tokenGenerator.GenerateToken(model);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            var tokenModel = new TokenModel()
            {
                Nickname = model.Nickname,
                Token = accessToken,
                RefreshToken = refreshToken
            };

            await _tokenRepository.Register(tokenModel);

            return new APIResponse<TokenModel>(tokenModel);
        }

        public async Task<APIResponse<TokenModel>> RefreshToken(TokenModel model)
        {
            var tokenModel = await _tokenRepository.GetByNickname(model.Nickname);

            if (tokenModel is null)
                return new APIResponse<TokenModel>(404, "Token is not found, try sign in again");

            if (tokenModel.Token != model.Token || tokenModel.RefreshToken != model.RefreshToken)
                return new APIResponse<TokenModel>(400, "Invalid refresh/access token");

            var claimsPrincipal = _tokenGenerator.GetClaimsPrincipal(model.Token);

            if(claimsPrincipal is null)
                return new APIResponse<TokenModel>(400, "Invalid access token");

            var userModel = new UserModel()
            {
                Nickname = claimsPrincipal.Identity.Name,
                Role = claimsPrincipal.Claims.ToList()[1].Value
            };

            var newAccessToken = _tokenGenerator.GenerateToken(userModel);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            return new APIResponse<TokenModel>(new TokenModel()
            {
                Nickname = userModel.Nickname,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });

        }


        

    }
}
