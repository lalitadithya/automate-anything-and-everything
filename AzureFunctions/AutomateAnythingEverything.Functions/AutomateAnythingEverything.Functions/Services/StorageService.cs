using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
            CloudBlob blob = GetBlob(fileName, containerName);

            using MemoryStream blobContents = new MemoryStream();
            await blob.DownloadToStreamAsync(blobContents);
            return Encoding.UTF8.GetString(blobContents.ToArray());
        }

        public string GetUriForFile(string fileName, string containerName)
        {
            CloudBlob blob = GetBlob(fileName, containerName);

            SharedAccessBlobPolicy sharedAccessBlobPolicy = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                Permissions = SharedAccessBlobPermissions.Read
            };

            var blobToken = blob.GetSharedAccessSignature(sharedAccessBlobPolicy);

            return blob.Uri + blobToken;
        }

        private CloudBlob GetBlob(string fileName, string containerName)
        {
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            var storageContainer = blobClient.GetContainerReference(containerName);
            var blob = storageContainer.GetBlobReference(fileName);
            return blob;
        }
    }
}
