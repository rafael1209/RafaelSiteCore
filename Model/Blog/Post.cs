using MongoDB.Bson;
using RafaelSiteCore.Model.Users;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Blog
{
        public class Post
        {
                public ObjectId Id { get; set; }

                public string Text { get; set; } = string.Empty;

                public string ImgUrl { get; set; } = string.Empty;

                public DateTime CretaedAtUtc { get; set; }

                public DateTime UpdatedAtUtc { get; set; }

                public ObjectId AuthorSearchToken { get; set; }

                public List<Comment> Comments { get; set; } = new List<Comment>();

                public List<Account> Likes { get; set; } = new List<Account>();
        }
}
