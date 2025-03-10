﻿using RafaelSiteCore.Model.Users;
using x3rt.DiscordOAuth2;
using x3rt.DiscordOAuth2.Entities.Enums;

namespace RafaelSiteCore.Services.Auth
{
        public class DiscordApiClient(string clientId, string clientSecret, string redirectUrl)
        {
            internal User GetUserInfo(string code)
                {
                        User user = new User();

                        Console.WriteLine("Client ID: "+clientId);
                        DiscordOAuth.Configure(ulong.Parse(clientId), clientSecret);

                        var scopes = new ScopesBuilder(OAuthScope.Identify);
                        var oAuth = new DiscordOAuth(redirectUrl, scopes);

                        var token = oAuth.GetTokenAsync(code).Result;

                        if (token != null)
                        {
                                var userDiscord = oAuth.GetUserAsync(token.AccessToken).Result;

                                if (userDiscord != null)
                                {
                                        user.DiscordId = userDiscord.Id;
                                        user.Name = userDiscord.Username;
                                        if (userDiscord.Avatar != null)
                                                user.AvatarUrl = $"https://cdn.discordapp.com/avatars/{user.DiscordId}/{userDiscord.Avatar}.png";
                                }
                        }

                        return user;
                }
        }
}
