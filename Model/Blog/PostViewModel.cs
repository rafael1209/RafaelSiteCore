using RafaelSiteCore.Model.Users;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Blog
{
        public class PostViewModel
        {
                [JsonPropertyName("SearchToken")]
                public string Id { get; set; } = string.Empty;

                public string Text { get; set; } = string.Empty;

                public string Imgurl { get; set; } = string.Empty;

                public DateTime CreatedAtUtc { get; set; }

                public DateTime UpdatedAtUtc { get; set; }

                public Account Account { get; set; } = new Account();

                public List<Comment> Comments { get; set; } = new List<Comment>();

                public int Likes { get; set; }
        }
}
