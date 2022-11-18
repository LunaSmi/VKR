using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models.Attach;
using VKR.API.Services;
using VKR.Common.Const;
using VKR.Common.Extensions;


namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "API")]
    [Authorize]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachService;
        private readonly UsersService _usersService;
        private readonly PostsService _postsService;

        public AttachController(AttachService attachService,
            UsersService usersService,
            PostsService postsService)
        {
            _attachService = attachService;
            _usersService = usersService;
            _postsService = postsService;
        }

        [HttpPost]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            var result = new List<MetadataModel>();
            foreach (var file in files)
            {
                result.Add(await _attachService.UploadFile(file));
            }
            return result;
        }

        [HttpPost]
        public async Task AddAvatarToUser(MetadataModel metaData)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.UserId);
            var tempFileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), metaData.TempId.ToString()));
            if (!tempFileInfo.Exists)
            {
                throw new Exception("File is not found");
            }
            else
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Files", metaData.TempId.ToString());

                var destFileInfo = new FileInfo(path);
                if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
                    destFileInfo.Directory.Create();

                System.IO.File.Copy(tempFileInfo.FullName, path, true);
                await _usersService.AddAvatarToUser(userId, metaData, path);
            }

        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<FileStreamResult> GetUserAvatar(Guid userId, bool download = false)
            => RenderAttach(await _usersService.GetUserAvatar(userId), download);

        [HttpGet]
        public async Task<FileStreamResult> GetCurentUserAvatar(bool download = false)
            => await GetUserAvatar(User.GetClaimValue<Guid>(ClaimNames.UserId), download);

        [HttpGet]
        [Route("{postContentId}")]
        public async Task<FileStreamResult> GetPostContent(Guid postContentId, bool download = false)
            => RenderAttach(await _postsService.GetPostContent(postContentId), download);





        private FileStreamResult RenderAttach(AttachModel attach, bool download)
        {
            var fileStream = new FileStream(attach.FilePath, FileMode.Open);
            var ext = Path.GetExtension(attach.Name);
            if (download)
                return File(fileStream, attach.MimeType, $"{attach.Id}{ext}");
            else
                return File(fileStream, attach.MimeType);

        }

    }
}
