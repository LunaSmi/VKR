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

        public UsersController(UsersService usersService,
            LinkGeneratorService links)
        {
            _usersService = usersService;

            links.LinkAvatarGenerator=x=> 
                Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
                {
                    userId = x.Id,
                });
        }


        [HttpGet]
        public async Task<UserAvatarModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            return await _usersService.GetUser(userId);
        }

        [HttpGet]
        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
            => await _usersService.GetUsers();

    }
}
