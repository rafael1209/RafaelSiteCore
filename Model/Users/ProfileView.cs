using RafaelSiteCore.Model.Blog;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Users
{
        public class ProfileView
        {
                public Account Account { get; set; } = new Account();

                public bool IsBanned { get; set; }

                public bool IsVerified { get; set; }

                public bool IsAuthor { get; set; }

                public int Followers { get; set; }

                public int Following {  get; set; }

                public bool IsFollowed { get; set; }

                public DateTime CreatedAtUtc { get; set; }

                public List<PostView> Posts { get; set; } = new List<PostView> { };
        }
}
