using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using IRFestival.Api.Common;
using IRFestival.Api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Web;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {

        private BlobUtility _blob;
        private IConfiguration _config;

        public PicturesController(BlobUtility blob, IConfiguration configuration)
        {
            _blob = blob;
            _config = configuration;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string[]))]
        public string[] GetAllPictureUrls()
        {
            var container = _blob.GetThumbsContainer();
            return container.GetBlobs()
                            .Select(x => _blob.GetSasUri(container, x.Name))
                            .ToArray();
        }

        [HttpPost]
        public async Task<ActionResult> PostPicture(IFormFile file)
        {
            BlobContainerClient container = _blob.GetPicturesContainer();
            var filename = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{HttpUtility.UrlPathEncode(file.FileName)}";
            await container.UploadBlobAsync(filename, file.OpenReadStream());

            await using (var client =  new ServiceBusClient(_config.GetConnectionString("ServiceBusSenderConnection")))
            {
                Mail mail = new()
                {
                    Body = $"The picture {filename} was uploaded! Send fictional email to me@you.us",
                    To = "dario.despi49@gmail.com"
                };
                var json = JsonSerializer.Serialize(mail);

                ServiceBusSender sender = client.CreateSender(_config.GetValue<string>("QueueNameMails"));
                ServiceBusMessage message = new(Encoding.UTF8.GetBytes(json));
                using(var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("dario.despi49@gmail.com", "goxnoxqgftcdzyqp");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mail.To, mail.To, "Test", mail.Body);
                }
                //await sender.SendMessageAsync(message);
            }


            return Ok();
        }
    }
}
