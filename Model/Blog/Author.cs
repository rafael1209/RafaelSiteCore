using MongoDB.Bson;

namespace RafaelSiteCore.Model.Blog
{
        public class Author
        {
                public string Name { get; set; } = string.Empty;

                public ulong DiscordId { get; set; }
        }
}
