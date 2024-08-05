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

                public void InfoLogger(string title,string description)
                {
                        SendWebhook("Info", title, description, Color.White);
                }

                public void ErrorLogger(string title, string description)
                {
                        SendWebhook("Error", title, description, Color.Red);
                }

                public void WarningLogger(string title, string description)
                {
                        SendWebhook("Warning", title, description, Color.Orange);
                }

                public void SendWebhook(string name, string title, string description ,Color color)
                {
                        try
                        {
                                DiscordWebhook hook = new DiscordWebhook
                                {
                                        Uri = new Uri(_webhookUrl)
                                };

                                DiscordMessage message = new DiscordMessage
                                {
                                        Username = name,
                                        AvatarUrl = new Uri("https://cdn.discordapp.com/avatars/293977705815343105/6856dc3a82fb70bb931566590f0a24ed.png")
                                };

                                DiscordEmbed embed = new DiscordEmbed
                                {
                                        Title = title,
                                        Description = description,
                                        Color = new DiscordColor(color),
                                        Footer = new EmbedFooter() { Text = "rafaelchasman.ru",IconUrl = new Uri("https://www.rafaelchasman.ru/_nuxt/diagrams-icon.BBdHJODi.png") },
                                        Timestamp = new DiscordTimestamp(DateTime.UtcNow),
                                }; 

                                message.Embeds.Add(embed);

                                hook.SendAsync(message);
                        }
                        catch (Exception ex)
                        {
                                Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                }
        }
}
