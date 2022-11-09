using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models;
using VKR.API.Models.Attach;
using VKR.API.Services;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachService;

        public AttachController(AttachService attachService)
        {
            _attachService = attachService;
        }


        [HttpPost]
        public async Task<List<MetaDataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            var result = new List<MetaDataModel>();
            foreach (var file in files)
            {
                result.Add(await _attachService.UploadFile(file));
            }
            return result;
        }
    }
}
