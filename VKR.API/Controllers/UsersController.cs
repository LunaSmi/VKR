using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VKR.API.Models;
using VKR.API.Models.Attach;
using VKR.API.Models.User;
using VKR.API.Services;
using VKR.DAL;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
                Url.Action(nameof(GetUserAvatar), new { userId = x.Id, download = false }));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<FileResult> GetUserAvatar(Guid userId)
        {
            var attach = await _usersService.GetUserAvatar(userId);
            return File(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType);
        }

        [HttpGet]
        public async Task<FileResult> DownloadAvatar(Guid userId)
        {
            var attach = await _usersService.GetUserAvatar(userId);

            HttpContext.Response.ContentType = attach.MimeType;
            FileContentResult result = new (System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType)
            {
                FileDownloadName = attach.Name
            };

            return result;
        }

        [HttpPost]
        [Authorize]
        public async Task AddAvatarToUser(MetaDataModel metaData)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var tempFileInfo = new FileInfo(Path.Combine(Path.GetTempPath(),metaData.TempId.ToString()));
                if (!tempFileInfo.Exists)
                {
                    throw new Exception("File is not found");
                }
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(),"Files",metaData.TempId.ToString());

                    var destFileInfo = new FileInfo(path);
                    if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
                        destFileInfo.Directory.Create();

                    System.IO.File.Copy(tempFileInfo.FullName, path, true);
                    await _usersService.AddAvatarToUser(userId, metaData,path);

                }
            }
            else
            {
                throw new Exception("You are not authorized");
            }

        }

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            var userIdString = User.Claims.FirstOrDefault(c=>c.Type=="userId")?.Value;
            if(Guid.TryParse(userIdString, out var userId))
            {
                return await _usersService.GetUser(userId);
            }
            else
            {
                throw new Exception("You are not authorized");
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers()
        {
           return await _usersService.GetUsers();
        }

    }
}
