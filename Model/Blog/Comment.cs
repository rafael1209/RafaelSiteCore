using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RafaelSiteCore.Model.Users;

namespace RafaelSiteCore.Model.Blog
{
        public class Comment
        {
                public ObjectId Id { get; set; }

                public string Text { get; set; } = string.Empty;

                public DateTime CreatedAtUtc { get; set; }

                public ObjectId AuthorSearchToken { get; set; }
        }
}
