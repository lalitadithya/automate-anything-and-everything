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
        public StorageService(CloudStorageAccount cloudStorageAccount)
        {
            this.cloudStorageAccount = cloudStorageAccount;
        }

        public async Task<string> GetFileContents(string fileName, string containerName)
        {
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            var storageContainer = blobClient.GetContainerReference(containerName);
            var blob = storageContainer.GetBlobReference(fileName);

            using MemoryStream blobContents = new MemoryStream();
            await blob.DownloadToStreamAsync(blobContents);
            return Encoding.UTF8.GetString(blobContents.ToArray());
        }
    }
}
