namespace RafaelSiteCore.Model.GoogleDriveCredentials
{
        public class GoogleDriveCredentials
        {
                public string ClientId { get; set; } = string.Empty;
                public string ProjectId { get; set; } = string.Empty;
                public string AuthUri { get; set; } = string.Empty;
                public string TokenUri { get; set; } = string.Empty;
                public string AuthProvider { get; set; } = string.Empty;
                public string ClientSecret { get; set; } = string.Empty;
                public string[] RedirectUri { get; set; }
        }
}
