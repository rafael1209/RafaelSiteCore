using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Users
{
    public class User
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsVerified { get; set; }

        public bool IsBanned { get; set; }

        public string AuthToken { get; set; } = string.Empty;

        public ulong DiscordId { get; set; }

        public string Email { get; set; } = string.Empty;

        public double Balance { get; set; }

        public string AvatarUrl { get; set; } = string.Empty;

        public List<ObjectId> Followers { get; set; } = new List<ObjectId>();

        public List<ObjectId> Following { get; set; } = new List<ObjectId>();

        public DateTime LastLoginUtc { get; set; }

        public DateTime CreatedDateUtc { get; set; }
    }
}