using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly string _credentialsPath;
        private readonly IMemoryCache _cache;

        public GoogleDriveService(IMemoryCache cache, IWebHostEnvironment env)
        {
            _credentialsPath = Path.Combine(env.ContentRootPath, "credentials.json");
            _cache = cache;
        }

        public async Task DeleteFileAsync(string fileName, string parentFolderId)
        {
            // 1. Authenticate with Google Drive API
            var credentials = await GetCredentialsAsync();

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "MartyrGraveManagement",
            });

            try
            {
                // Find the file
                var request = service.Files.List();
                request.Q = $"name='{fileName}' and parents in '{parentFolderId}'";
                var files = await request.ExecuteAsync();

                if (files.Files.Count > 0)
                {
                    var fileId = files.Files[0].Id;

                    // Delete the file
                    await service.Files.Delete(fileId).ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Some error with it, please contact administrator");
            }
        }

        public async Task<byte[]> GetFileContentAsync(string fileName, string parentFolderId)
        {
            // 1. Authenticate with Google Drive API
            var credentials = await GetCredentialsAsync();

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "MartyrGraveManagement",
            });

            try
            {
                // Find the file
                var request = service.Files.List();
                request.Q = $"name='{fileName}' and parents in '{parentFolderId}'";
                var files = await request.ExecuteAsync();

                if (files.Files.Count > 0)
                {
                    var fileId = files.Files[0].Id;

                    // Get the file content
                    var memoryStream = new MemoryStream();
                    var getRequest = service.Files.Get(fileId);
                    await getRequest.DownloadAsync(memoryStream);
                    return memoryStream.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Some error with it, please contact administrator", ex);
            }
        }

        public async Task<bool> UploadFileAsync(Stream fileStream, Google.Apis.Drive.v3.Data.File fileMetadata)
        {
            try
            {

                // 1. Authenticate with Google Drive API
                var credentials = await GetCredentialsAsync();

                // Create the DriveService instance
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials,
                    ApplicationName = "MartyrGraveManagement",
                });

                // Upload the file using the stream
                var file = await service.Files.Create(fileMetadata, fileStream, fileMetadata.MimeType)
                    .UploadAsync();
                if (file.Status == UploadStatus.Completed)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"File upload failed: {file.Exception?.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<UserCredential> GetCredentialsAsync()
        {
            try
            {
                string credentialsPath = "D:\\Capstone Project\\Group Project\\Google Drive\\credentials.json"; //Đường dẫn file credentials.json
                // Set up the flow and the data store
                using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { DriveService.Scope.DriveFile },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("GoogleDriveUploads.json", true)
                ).Result;
                return credential;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> GetVideoFromCacheOrDriveAsync(string imagePath, string parentFolderId)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    return null;
                }

                string cacheKey = $"{imagePath}";
                if (!_cache.TryGetValue(cacheKey, out byte[] imageContent))
                {
                    // Image not in cache, fetch from Google Drive
                    imageContent = await GetFileContentAsync(imagePath, parentFolderId);

                    if (imageContent != null)
                    {
                        // Store in cache for 1 hour
                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                        };
                        _cache.Set(cacheKey, imageContent, cacheEntryOptions);
                    }
                }

                return imageContent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetFileIdByNameAsync(string fileName, string parentFolderId)
        {
            var credentials = await GetCredentialsAsync();
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = "MartyrGraveManagement",
            });

            var request = service.Files.List();
            request.Q = $"name='{fileName}' and parents in '{parentFolderId}'";
            var files = await request.ExecuteAsync();

            if (files.Files.Count > 0)
            {
                return files.Files[0].Id;
            }

            return null;
        }
    }
}
