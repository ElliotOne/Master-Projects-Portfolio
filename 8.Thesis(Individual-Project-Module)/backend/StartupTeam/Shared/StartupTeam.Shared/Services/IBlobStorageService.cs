namespace StartupTeam.Shared.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadBlobAsync(string containerName, string blobName, Stream stream);
        Task<bool> DeleteBlobAsync(string containerName, string blobName);
    }
}
