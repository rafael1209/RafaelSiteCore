using MongoDB.Bson;

namespace RafaelSiteCore.Model.Users
{
        public class Account
        {
                public string SearchToken { get; set; } = string.Empty;

                public string Username { get; set; } = string.Empty;

                public string AvatarUrl { get; set; } = string.Empty;

                public bool IsVerified { get; set; }
        }
}
