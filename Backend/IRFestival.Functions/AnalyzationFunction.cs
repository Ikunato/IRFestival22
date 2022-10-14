using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace IRFestival.Functions
{
    public class AnalyzationFunction
    {

        public ComputerVisionClient Client { get; }
        private static readonly List<VisualFeatureTypes?> Features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Adult };

        public AnalyzationFunction(ComputerVisionClient client)
        {
            Client = client;
        }

        [FunctionName("AnalyzationFunction")]
        public async Task Run([BlobTrigger("festivalpics-uploaded/{name}", Connection = "BlobStorageConnection")]byte[] myBlob,
            string name, ILogger log, Binder binder)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            ImageAnalysis analysis = await Client.AnalyzeImageInStreamAsync(new MemoryStream(myBlob), Features);

            Attribute[] attributes;
            string blobPath;
            if (analysis.Adult.IsAdultContent || analysis.Adult.IsRacyContent || analysis.Adult.IsGoryContent)
                blobPath = $"festivalpics-rejected/{name}";
            else 
                blobPath = $"festivalpics-approved/{name}";
            attributes = new Attribute[]
                {
                    new BlobAttribute(blobPath, FileAccess.Write),
                    new StorageAccountAttribute("BlobStorageConnection")
                };
            using Stream fileOutputStream = await binder.BindAsync<Stream>(attributes);
            fileOutputStream.Write(myBlob);

        }
    }
}
