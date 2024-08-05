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

                public async Task TestAsync(string name)
                {
                        try
                        {
                                DiscordWebhook hook = new DiscordWebhook
                                {
                                        Uri = new Uri("https://discord.com/api/webhooks/1270135141938761891/LGtWnHmC-CcRyMmPve2luGAVHbj-W_kiLIohjdG0YGPJ2JUsZaiaQNaRPfcpSYD6SRLg")
                                };

                                DiscordMessage message = new DiscordMessage
                                {
                                        Username = name,
                                        AvatarUrl = new Uri("https://cdn.discordapp.com/avatars/293977705815343105/6856dc3a82fb70bb931566590f0a24ed.png")
                                };

                                DiscordEmbed embed = new DiscordEmbed
                                {
                                        Title = "Embed title",
                                        Description = "sam",
                                };

                                message.Embeds.Add(embed);

                                await hook.SendAsync(message);
                        }
                        catch (Exception ex)
                        {
                                Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                }
        }
}
