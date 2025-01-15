namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IGoogleDriveService
    {
        Task<bool> UploadFileAsync(Stream fileStream, Google.Apis.Drive.v3.Data.File fileMetadata);
        Task DeleteFileAsync(string fileName, string parentFolderId);
        Task<byte[]> GetFileContentAsync(string fileName, string parentFolderId);
        Task<byte[]> GetVideoFromCacheOrDriveAsync(string imagePath, string parentFolderId);
        Task<string> GetFileIdByNameAsync(string fileName, string parentFolderId);
    }
}
