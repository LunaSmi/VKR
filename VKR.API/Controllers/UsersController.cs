using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models.Attach;
using VKR.API.Models.User;
using VKR.API.Services;
using VKR.Common.Const;
using VKR.Common.Extensions;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "API")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(
            UsersService usersService)
        {
            _usersService = usersService;
            if (usersService != null)
                _usersService.SetLinkGenerator(x =>
                Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
                {
                    userId = x.Id,
                }));
        }


        [HttpGet]
        public async Task<UserAvatarModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            if (userId != default)
            {
                return await _usersService.GetUser(userId);
            }
            else
            {
                throw new Exception("You are not authorized");
            }
        }

        [HttpGet]
        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
        {
            return await _usersService.GetUsers();
        }

    }
}
