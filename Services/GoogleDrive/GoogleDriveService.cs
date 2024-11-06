using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using RafaelSiteCore.Model.GoogleDriveCredentials;
using System.Text;
using RafaelSiteCore.Interfaces;
using Google.Apis.Upload;
using SharpCompress.Common;

namespace RafaelSiteCore.Services.GoogleDrive
{
        public class GoogleDriveService : IStorage
        {
                private readonly DriveService _driveService;

                private string _googleDriveFolderId;

                public GoogleDriveService(GoogleDriveCredentials googleDriveCredential, string driveolderId)
                {

                        _googleDriveFolderId = driveolderId;

                        try
                        {
                                string jsonString = $@"
                                {{
                                        ""type"": ""{googleDriveCredential.Type}"",
                                        ""project_id"": ""{googleDriveCredential.ProjectId}"",
                                        ""private_key_id"": ""{googleDriveCredential.PrivateKeyId}"",
                                        ""private_key"": ""{googleDriveCredential.PrivateKey}"",
                                        ""client_email"": ""{googleDriveCredential.ClientEmail}"",
                                        ""client_id"": ""{googleDriveCredential.ClientId}"",
                                        ""auth_uri"": ""{googleDriveCredential.AuthUri}"",
                                        ""token_uri"": ""{googleDriveCredential.TokenUri}"",
                                        ""auth_provider_x509_cert_url"": ""{googleDriveCredential.AuthProviderX509CertUrl}"",
                                        ""client_x509_cert_url"": ""{googleDriveCredential.ClientX509CertUrl}"",
                                        ""universe_domain"": ""{googleDriveCredential.UniverseDomain}""
                                }}";

                                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(jsonString);
                                MemoryStream stream = new MemoryStream(byteArray);

                                GoogleCredential credential = GoogleCredential.FromStream(stream).CreateScoped(new[]
                                 {
                                        DriveService.ScopeConstants.DriveFile
                                 });

                                _driveService = new DriveService(new BaseClientService.Initializer()
                                {
                                        HttpClientInitializer = credential,
                                        ApplicationName = "SpDonateFileStorage"
                                });
                        }
                        catch (Exception ex)
                        {
                                Console.WriteLine($"Error initializing Google Drive service: {ex.Message}");
                                throw;
                        }
                }

                public async Task<string> UploadFile(IFormFile file)
                {
                        var allowedExtensions = new[] { ".png", ".gif", ".webp", ".jpeg", ".jpg" };
                        var extension = Path.GetExtension(file.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                                return await Task.FromResult("Invalid file type. Only PNG and GIF files are allowed.");
                        }

                        try
                        {
                                var folderId = _googleDriveFolderId;

                                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                                {
                                        Name = file.FileName,
                                        MimeType = file.ContentType,
                                        Parents = new[] { folderId }
                                };

                                FilesResource.CreateMediaUpload request;

                                using (var stream = file.OpenReadStream())
                                {
                                        request = _driveService.Files.Create(fileMetadata, stream, "");
                                        request.Fields = "id";
                                        var response = request.Upload();
                                        if (response.Status == UploadStatus.Completed)
                                        {
                                        }
                                }

                                var fileId = request.ResponseBody?.Id;

                                var permission = new Google.Apis.Drive.v3.Data.Permission
                                {
                                        Role = "reader",
                                        Type = "anyone"
                                };
                                await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();

                                return await Task.FromResult($"https://lh3.googleusercontent.com/d/{fileId}");
                        }
                        catch (Exception)
                        {
                                return await Task.FromResult($"Error");
                        }
                }
        }
}