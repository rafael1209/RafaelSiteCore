using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using CSharpDiscordWebhook.NET.Discord;
using System;
using System.Drawing;
using System.Xml.Linq;
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

                public void InfoLogger(string title, string description, string uri)
                {
                        SendWebhook("Info", title, description, uri, Color.White);
                }

                public void ErrorLogger(string title, string description, string uri)
                {
                        SendWebhook("Error", title, description, uri, Color.Red);
                }

                public void WarningLogger(string title, string description, string uri)
                {
                        SendWebhook("Warning", title, description, uri, Color.Orange);
                }

                public void SendNewPostAlert()
                {
                        List<string> messages = new List<string>();
                        Random random = new Random();
                        try
                        {
                                DiscordWebhook hook = new DiscordWebhook
                                {
                                        Uri = new Uri("https://discord.com/api/webhooks/1270170672647241818/BqxqBKLnlxayzk6kHlGo1wgdk7rjzA2yiVbWUSjC6um8qPZWzBlvJipfhpaz7ZvjKCHy")
                                };

                                DiscordMessage message = new DiscordMessage
                                {
                                        Username = "Alert",
                                        AvatarUrl = new Uri("https://cdn.discordapp.com/avatars/293977705815343105/6856dc3a82fb70bb931566590f0a24ed.png")
                                };

                                DiscordEmbed embed = new DiscordEmbed
                                {
                                        Title = "New Post!",
                                        Description = "Иди сука смотри, новый пост на сайте!",
                                        Color = new DiscordColor(Color.AliceBlue),
                                        Footer = new EmbedFooter() { Text = "rafaelchasman.ru", IconUrl = new Uri("https://www.rafaelchasman.ru/_nuxt/diagrams-icon.BBdHJODi.png") },
                                        Timestamp = new DiscordTimestamp(DateTime.UtcNow)
                                };

                                message.Embeds.Add(embed);

                                hook.SendAsync(message);
                        }
                        catch (Exception ex)
                        {
                                Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                }

                public void SendWebhook(string name, string title, string description, string avatarUri, Color color)
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
                                        Footer = new EmbedFooter() { Text = "rafaelchasman.ru", IconUrl = new Uri("https://www.rafaelchasman.ru/_nuxt/diagrams-icon.BBdHJODi.png") },
                                        Timestamp = new DiscordTimestamp(DateTime.UtcNow),
                                        Thumbnail = new EmbedMedia() { Url = new Uri(avatarUri) }
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
