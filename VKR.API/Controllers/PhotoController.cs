using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models;
using VKR.API.Services;
using VKR.DAL.Entities;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly PhotoService _photoService;
        private readonly PostService _postService;

        public PhotoController(PhotoService photoService,
            PostService postService)
        {
            _photoService = photoService;
            _postService = postService;
        }

        //[HttpGet]
        //public async Task<FileResult> GetPostPhotos(Guid postId)
        //{
        //    var attach = await _postService.GetPostPhotos(postId);
        //    return File(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType);
        //}






    }
}
