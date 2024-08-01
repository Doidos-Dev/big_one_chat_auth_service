using Application.Responses;
using Application.Services.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;


namespace BIgOneChatAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("token")]
        public async Task<ActionResult<APIResponse<TokenModel>>> GenerateNewToken(UserModel model)
            => await _authService.GenerateToken(model);

        [HttpPost("token/refresh")]
        public async Task<ActionResult<APIResponse<TokenModel>>> Refresh(TokenModel model)
            => await _authService.RefreshToken(model);
        
    }
}
