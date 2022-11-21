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
            links.LinkAvatarForCommentGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
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
        public async Task<List<PostModel>> GetPostsForCurrentUser(int skip = 0, int take = 10)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            return await _postsService.GetPosts(userId,skip, take);
        }

        [HttpGet]
        public async Task<PostModel> GetPostByIdForCurrentUser(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            return await _postsService.GetPostById(postId,userId);
        }


    }
}
