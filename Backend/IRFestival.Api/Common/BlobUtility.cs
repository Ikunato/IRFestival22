using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using IRFestival.Api.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRFestival.Api.Common
{
    public class BlobUtility
    {
        private StorageSharedKeyCredential Credential { get; set; }
        private BlobServiceClient Client;
        private BlobSettingsOptions Options;

        public BlobUtility(StorageSharedKeyCredential credential,
            BlobServiceClient client, IOptions<BlobSettingsOptions> options)
        {
            Credential = credential;
            Client = client;
            Options = options.Value;
        }

        public BlobContainerClient GetPicturesContainer() => Client.GetBlobContainerClient(Options.PicturesContainer);

        public string GetSasUri(BlobContainerClient container, string name)
        {
            BlobSasBuilder sasBuilder = new()
            {
                BlobContainerName = container.Name,
                BlobName = name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2),
            };
            sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
            string sasToken = sasBuilder.ToSasQueryParameters(Credential).ToString();

            return $"{container.Uri.AbsoluteUri}/{name}?{sasToken}";

        }
    }
}
