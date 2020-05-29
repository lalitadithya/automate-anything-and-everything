using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AutomateAnythingEverything.Functions.Services
{
    public class StorageService
    {
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly IConfigurationRoot configuration;
        public StorageService(CloudStorageAccount cloudStorageAccount, IConfigurationRoot configuration)
        {
            this.cloudStorageAccount = cloudStorageAccount;
            this.configuration = configuration;
        }

        public async Task<string> GetFileContents(string fileName)
        {
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            var storageContainer = blobClient.GetContainerReference(configuration["ContainerName"]);
            var blob = storageContainer.GetBlobReference(fileName);

            using MemoryStream blobContents = new MemoryStream();
            await blob.DownloadToStreamAsync(blobContents);
            return Encoding.UTF8.GetString(blobContents.ToArray());
        }
    }
}
