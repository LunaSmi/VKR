using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models;
using VKR.API.Models.Post;
using VKR.API.Services;
using VKR.DAL.Entities;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly UsersService _usersService;

        public PostController(PostService postService, 
            UsersService usersService)
        {
            _postService = postService;
            _usersService = usersService;

        }



        [HttpPost]
        [Authorize]
        public async Task<Guid> CreatePost(CreatePostModel post)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var result = await _postService.CreatePost(post,userId);
                return result;
            }
            else
            {
                throw new Exception("You are not authorized");
            }

        }



    }
}
