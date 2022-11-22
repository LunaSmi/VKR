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
    public class CommentsController : ControllerBase
    {
        private readonly PostsService _postsService;

        public CommentsController(PostsService postsService, LinkGeneratorService links)
        {
            _postsService = postsService;
            links.LinkAvatarForCommentGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
        }

        [HttpPost]
        public async Task AddCommentToPost(CreateCommentRequest commentRequest)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            commentRequest.AuthorId=userId;
            await _postsService.AddCommentToPost(commentRequest);
        }

        [HttpGet]
        public async Task<List<CommentModel>> GetAllCommentsByPostId(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            return await _postsService.GetAllCommentsByPostId(postId, userId);
        }



    }
}
