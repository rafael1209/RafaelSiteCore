using MongoDB.Bson;

namespace RafaelSiteCore.Model.Blog
{
        public class PostSummary
        {
                public string PostId { get; set; } = string.Empty;

                public string Author { get; set; } = string.Empty;

                public string Title { get; set; } = string.Empty;

                public string Body { get; set; } = string.Empty;

                public string Imgurl { get; set; } = string.Empty;

                public int Likes { get; set; }

                public DateTime CretaedAtUtc { get; set; }
        }
}
