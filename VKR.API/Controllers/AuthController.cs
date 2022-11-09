using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models;
using VKR.API.Models.Token;
using VKR.API.Models.User;
using VKR.API.Services;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UsersService _usersService;

        public AuthController(UsersService usersService)
        {
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
            =>await _usersService.GetTokens(model.Login,model.Password);

        [HttpPost]
        public async Task<TokenModel> RefreshToken(RefreshTokenRequestModel model) 
            => await _usersService.GetTokensByRefreshToken(model.RefreshToken);

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var sessionIdString = User.Claims.FirstOrDefault(c => c.Type == "sessionId")?.Value;
            if (Guid.TryParse(sessionIdString, out var sessionId))
            {
                await _usersService.RemoveSession( sessionId);
                return Ok("You are logout");
            }
            else
            {
                return BadRequest("You are not authorized");
            }
        }


    }
}
