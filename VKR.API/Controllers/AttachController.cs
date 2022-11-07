﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VKR.API.Models;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {

        [HttpPost]
        public async Task<List<MetaDataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            var result = new List<MetaDataModel>();
            foreach (var file in files)
            {
                result.Add(await UploadFile(file));
            }
            return result;
        }

        private static async Task<MetaDataModel> UploadFile(IFormFile file)
        {
            var tempPath = Path.GetTempPath();
            var meta = new MetaDataModel
            {
                TempId=Guid.NewGuid(),
                Name = file.FileName,
                MimeType=file.ContentType,
                Size=file.Length
            };

            var newPath = Path.Combine(tempPath, meta.TempId.ToString());

            var fileinfo = new FileInfo(newPath);
            if (fileinfo.Exists)
            {
                throw new Exception("file exist");
            }
            else
            {
                if (fileinfo.Directory == null)
                {
                    throw new Exception("temp is null");
                }
                else
                if (!fileinfo.Directory.Exists)
                {
                    fileinfo.Directory?.Create();
                }

                using (var stream = System.IO.File.Create(newPath))
                {
                    await file.CopyToAsync(stream);
                }

                return meta;
            }
        }

    }
}