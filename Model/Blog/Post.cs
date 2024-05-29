using MongoDB.Bson;
using RafaelSiteCore.Model.Users;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Blog
{
        public class Post
        {
                [JsonPropertyName("SearchToken")]
                public ObjectId Id { get; set; }

                public Account Author { get; set; } = new Account();

                public string Title { get; set; } = string.Empty;

                public string body { get; set; } = string.Empty;

                public string ImgUrl { get; set; } = string.Empty;

                public List<User> Likes { get; set; } = new List<User>();

                public DateTime CreatedDateUtc { get; set; }
        }
}
