using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using RafaelSiteCore.Model.GoogleDriveCredentials;
using System.Text;
using RafaelSiteCore.Interfaces;

namespace RafaelSiteCore.Services.GoogleDrive
{
        public class GoogleDriveService : IStorage
        {
                private readonly DriveService _driveService;

                private string _googleDriveFolderId;

                public GoogleDriveService(GoogleDriveCredentials googleDriveCredential, string driveolderId)
                {
                        UserCredential credential;

                        _googleDriveFolderId = driveolderId;

                        try
                        {
                                string credentialsJson = $@"{{
                                        'installed': {{
                                                'client_id': '{googleDriveCredential.ClientId}',
                                                'project_id': '{googleDriveCredential.ProjectId}',
                                                'auth_uri': '{googleDriveCredential.AuthUri}',
                                                'token_uri': '{googleDriveCredential.TokenUri}',
                                                'auth_provider_x509_cert_url': '{googleDriveCredential.AuthProvider}',
                                                'client_secret': '{googleDriveCredential.ClientSecret}',
                                                'redirect_uris': '{googleDriveCredential.RedirectUri}'
                                        }}
                                }}";

                                var credentialStream = new MemoryStream(Encoding.UTF8.GetBytes(credentialsJson));
                                var clientSecrets = GoogleClientSecrets.FromStream(credentialStream);

                                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                                    clientSecrets.Secrets,
                                    new[] { DriveService.ScopeConstants.DriveFile },
                                        "user",
                                    CancellationToken.None).Result;

                                _driveService = new DriveService(new BaseClientService.Initializer()
                                {
                                        HttpClientInitializer = credential,
                                        ApplicationName = "Your Application Name",
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
                        if (file == null || file.Length == 0)
                        {
                                return "No file selected.";
                        }

                        var allowedExtensions = new[] { ".png", ".gif" };
                        var extension = Path.GetExtension(file.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                                return "Invalid file type. Only PNG and GIF files are allowed.";
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
                                        request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
                                        request.Fields = "id";
                                        await request.UploadAsync();
                                }

                                var fileId = request.ResponseBody?.Id;

                                if (string.IsNullOrEmpty(fileId))
                                {
                                        return "Error uploading file.";
                                }

                                var permission = new Google.Apis.Drive.v3.Data.Permission
                                {
                                        Role = "reader",
                                        Type = "anyone"
                                };
                                await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();

                                string fileLink = $"https://drive.google.com/thumbnail?id={fileId}";

                                return $"{fileLink}";
                        }
                        catch (Exception ex)
                        {
                                return $"Error: {ex}";
                        }
                }

        }
}
