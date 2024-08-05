using CSharpDiscordWebhook.NET.Discord;
using System.Drawing;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace RafaelSiteCore.Services.Logger
{
        public class DiscordAlert
        {
                private readonly string _webhookUrl;

                public DiscordAlert(string webhookUrl)
                {
                        _webhookUrl = webhookUrl;
                }

                public async Task TestAsync()
                {
                        DiscordWebhook hook = new DiscordWebhook();
                        hook.Uri = new Uri(_webhookUrl);

                        DiscordMessage message = new DiscordMessage();
                        message.Content = "Example message, ping @everyone, <@userid>";
                        message.TTS = true; //read message to everyone on the channel
                        message.Username = "Webhook username";
                        message.AvatarUrl = new Uri("https://cdn.discordapp.com/avatars/293977705815343105/6856dc3a82fb70bb931566590f0a24ed.png");

                        //embeds
                        DiscordEmbed embed = new DiscordEmbed();
                        embed.Title = "Embed title";
                        embed.Description = "Embed description";
                        embed.Url = new Uri("Embed Url");
                        embed.Timestamp = new DiscordTimestamp(DateTime.Now);
                        embed.Color = new DiscordColor(Color.Red); //alpha will be ignored, you can use any RGB color
                        embed.Footer = new EmbedFooter() { Text = "Footer Text", IconUrl = new Uri("http://url-of-image") };
                        embed.Image = new EmbedMedia() { Url = new Uri("Media URL"), Width = 150, Height = 150 }; //valid for thumb and video
                        embed.Provider = new EmbedProvider() { Name = "Provider Name", Url = new Uri("Provider Url") };
                        embed.Author = new EmbedAuthor() { Name = "Author Name", Url = new Uri("Author Url"), IconUrl = new Uri("http://url-of-image") };

                        message.Embeds.Add(embed);

                        

                        await hook.SendAsync(message);
                }
        }
}
