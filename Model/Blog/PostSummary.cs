using MongoDB.Bson;
using RafaelSiteCore.Model.Users;

namespace RafaelSiteCore.Model.Blog
{
        public class PostSummary
        {
                public string PostId { get; set; } = string.Empty;

                public Account Author { get; set; } = new Account();

                public string Title { get; set; } = string.Empty;

                public string Body { get; set; } = string.Empty;

                public string Imgurl { get; set; } = string.Empty;

                public int Likes { get; set; }

                public DateTime CretaedAtUtc { get; set; }
        }
}
