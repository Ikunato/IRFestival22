using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace IRFestival.Functions
{
    public class FunctionThumbnail
    {
        [FunctionName("FunctionThumbnail")]

        public static void Run(
            [BlobTrigger("festivalpics-uploaded/{name}", Connection = "BlobStorageConnection")] Stream myBlob,
            string name,
            ILogger log,
            [Blob("festivalthumbs/{name}", FileAccess.Write, Connection = "BlobStorageConnection")] Stream thumbnail)
        {
            using Image<Rgba32> input = Image.Load<Rgba32>(myBlob, out IImageFormat format);
            input.Mutate(i =>
            {
                i.Resize(340, 0);
                int height = i.GetCurrentSize().Height;
                i.Crop(new Rectangle(0, 0, 340, height < 226 ? height : 226));
            });

            //Some comment
            input.Save(thumbnail, format);
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
