﻿using RafaelSiteCore.Model.Users;

namespace RafaelSiteCore.Model.Blog
{
        public class CommantView
        {
                public string Id { get; set; } = string.Empty;

                public string Text { get; set; } = string.Empty;

                public DateTime CreatedAtUtc { get; set; }

                public Account Account { get; set; } = new Account();

                public int Likes { get; set; }

                public bool IsLiked { get; set; }
        }
}
