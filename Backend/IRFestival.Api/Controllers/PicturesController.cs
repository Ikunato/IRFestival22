using Azure.Storage.Blobs;
using IRFestival.Api.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Web;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {

        private BlobUtility _blob;

        public PicturesController(BlobUtility blob)
        {
            _blob = blob;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string[]))]
        public async Task<ActionResult> GetAllPictureUrls()
        {
            var container = _blob.GetPicturesContainer();
            var result = container.GetBlobs()
                            .Select(x => _blob.GetSasUri(container, x.Name))
                            .ToArray();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> PostPicture(IFormFile file)
        {
            BlobContainerClient container = _blob.GetPicturesContainer();
            var filename = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{HttpUtility.UrlPathEncode(file.FileName)}";
            await container.UploadBlobAsync(filename, file.OpenReadStream());

            return Ok();
        }
    }
}
