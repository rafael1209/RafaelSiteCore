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

                public string Body { get; set; } = string.Empty;

                public string ImgUrl { get; set; } = string.Empty;

                public List<Account> Likes { get; set; } = new List<Account>();

                public DateTime CreatedDateUtc { get; set; }
        }
}
