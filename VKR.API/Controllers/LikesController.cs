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
            await _postsService.AddLikeToPost(postId, userId);
        }

        [HttpPost]
        public async Task RemoveLikeFromPost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            await _postsService.RemoveLikeFromPost(postId, userId);
        }

        [HttpPost]
        public async Task AddLikeToComment(Guid commentId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            await _postsService.AddLikeToComment(commentId, userId);
        }

        [HttpPost]
        public async Task RemoveLikeFromComment(Guid commentId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            await _postsService.RemoveLikeFromComment(commentId, userId);
        }


    }
}
