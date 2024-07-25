using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BIgOneChatAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("token")]
        public ActionResult<string> CreateToken(UserModel model)
            => TokenGenerator.GenerateToken(model);
    }
}
