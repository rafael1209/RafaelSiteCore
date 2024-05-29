namespace RafaelSiteCore.Model.Users
{
        public class Account
        {
                public ulong DiscordId { get; set; }

                public string Name { get; set; } = string.Empty;

                public string AvatarUrl { get; set; } = string.Empty;
        }
}
