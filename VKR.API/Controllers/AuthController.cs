using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models;
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
        public async Task<TokenModel> Login(TokenRequestModel model)
            =>await _usersService.GetTokens(model.Login,model.Password);

        [HttpPost]
        public async Task<TokenModel> RefreshToken(RefreshTokenRequestModel model) 
            => await _usersService.GetTokensByRefreshToken(model.RefreshToken);



    }
}
