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
    [ApiExplorerSettings(GroupName = "API")]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly PostsService _postsService;
        private readonly UsersService _usersService;

        public PostsController(PostsService postService, 
            UsersService usersService,
            LinkGeneratorService links)
        {
            _usersService = usersService;
            _postsService = postService;
            links.LinkContentGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostContent), new
            {
                postContentId =x.Id,
            });
            links.LinkAvatarGenerator = x=> Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId =x.Id,
            });
        }

        [HttpPost]
        public async Task CreatePost(CreatePostRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);

            request.AuthorId = userId;
            await _postsService.CreatePost(request);
        }


        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int skip = 0, int take = 10)
            => await _postsService.GetPosts(skip, take);

        [HttpGet]
        public async Task<PostModel> GetPostById(Guid id)
            => await _postsService.GetPostById(id);



    }
}
