using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models.Token;
using VKR.API.Models.User;
using VKR.API.Services;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName ="Auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UsersService _usersService;


        public AuthController(AuthService authService,
            UsersService usersService)
        {
            _authService = authService;
            _usersService = usersService;
        }


        [HttpPost]
        public async Task Register(CreateUserModel createUserModel)
        {
            if (await _usersService.CheckUserExistAsync(createUserModel.Email))
            {
                throw new Exception("user is exist");
            }
            await _usersService.CreateUser(createUserModel);
        }

        [HttpPost]
        public async Task<TokenModel> Login(TokenRequestModel model)
            => await _authService.GetTokens(model.Login, model.Password);

        [HttpPost]
        [Authorize]
        [ApiExplorerSettings(GroupName = "API")]
        public async Task<TokenModel> RefreshToken(RefreshTokenRequestModel model)
            => await _authService.GetTokensByRefreshToken(model.RefreshToken);

        [HttpPost]
        [Authorize]
        [ApiExplorerSettings(GroupName = "API")]
        public async Task<IActionResult> Logout()
        {
            var sessionIdString = User.Claims.FirstOrDefault(c => c.Type == "sessionId")?.Value;
            if (Guid.TryParse(sessionIdString, out var sessionId))
            {
                await _authService.RemoveSession(sessionId);
                return Ok("You are logout");
            }
            else
            {
                return BadRequest("You are not authorized");
            }
        }


    }
}
