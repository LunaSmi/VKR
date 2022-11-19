using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models.Post;
using VKR.API.Services;
using VKR.Common.Const;
using VKR.Common.Extensions;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "API")]
    [Authorize]
    public class LikesController : ControllerBase
    {
        private readonly PostsService _postsService;

        public LikesController(PostsService postsService)
        {
            _postsService = postsService;
        }

        [HttpPost]
        public async Task AddLikeToPost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            await _postsService.AddLikeToPostAsync(postId, userId);
        }

        [HttpPost]
        public async Task RemoveLikeFromPost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            await _postsService.RemoveLikeFromPostAsync(postId, userId);
        }




    }
}
