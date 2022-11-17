using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models.Post;
using VKR.API.Services;
using VKR.Common.Const;
using VKR.Common.Extensions;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly PostsService _postsService;
        private readonly UsersService _usersService;

        private string? _linkAvatarGenerator(Guid userId)
        {
            return Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId,
            });
        }
        private string? _linkContentGenerator(Guid postContentId)
        {
            return Url.ControllerAction<AttachController>(nameof(AttachController.GetPostContent), new
            {
                postContentId,
            });
        }

        public PostsController(PostsService postService, UsersService usersService)
        {
            _usersService = usersService;
            _postsService = postService;
            _postsService.SetLinkGenerator(_linkContentGenerator, _linkAvatarGenerator);
        }

        [HttpPost]
        public async Task CreatePost(CreatePostRequest request)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.UserId)?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                request.AuthorId = userId;
                await _postsService.CreatePost(request);
            }
            else
            {
                throw new Exception("You are not authorized");
            }

        }

        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int skip = 0, int take = 10)
            => await _postsService.GetPosts(skip, take);





    }
}
