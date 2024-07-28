using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using RafaelSiteCore.Model.Users;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Blog
{
        public class PostView
        {
                [JsonPropertyName("SearchToken")]
                public string Id { get; set; } = string.Empty;

                public string Text { get; set; } = string.Empty;

                public string ImgUrl { get; set; } = string.Empty;

                public DateTime CreatedAtUtc { get; set; }

                public DateTime UpdatedAtUtc { get; set; }

                public Account Account { get; set; } = new Account();

                public List<CommantViewModel> Comments { get; set; } = new List<CommantViewModel>();

                public int Likes { get; set; }
        }

        public class CommantViewModel
        {
                public string Id { get; set; } = string.Empty ;

                public string Text { get; set; } = string.Empty;

                public DateTime CreatedAtUtc { get; set; }

                public Account Account { get; set; } = new Account();
        }
}
