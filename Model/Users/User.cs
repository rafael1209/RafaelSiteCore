using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Users
{
        public class User
        {
                [JsonPropertyName("SearchToken")]
                public ObjectId Id { get; set; }

                public string Name { get; set; } = string.Empty;

                public string AuthToken { get; set; } = string.Empty;

                public ulong DiscordId { get; set; }

                public string Email { get; set; } = string.Empty;

                public double Balance { get; set; }

                public string AvatarUrl { get; set; } = string.Empty;

                public DateTime LastLoginUtc { get; set; }

                public DateTime CreatedDateUtc { get; set; }
        }
}
