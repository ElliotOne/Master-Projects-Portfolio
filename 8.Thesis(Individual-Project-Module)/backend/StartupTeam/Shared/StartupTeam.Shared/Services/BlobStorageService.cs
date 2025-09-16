using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace StartupTeam.Shared.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> DeleteBlobAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            return await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
