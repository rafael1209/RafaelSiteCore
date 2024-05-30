using RafaelSiteCore.Model.Users;
using x3rt.DiscordOAuth2;
using x3rt.DiscordOAuth2.Entities.Enums;

namespace RafaelSiteCore.Services.Auth
{
        public class DiscordApiClient
        {
                private readonly ulong _clientID;
                private readonly string _clientSecret;
                private readonly string _redirectUrl;

                public DiscordApiClient(ulong clientId, string clientSecret, string redirectUrl)
                {
                        this._clientID = /*1242498004070432908;*/ clientId;
                        this._clientSecret = /*"OcrEABaN00NIWtePmBfjnAS_Co4Iy7i8";*/ clientSecret;
                        this._redirectUrl = /*"https://rafaelchasman.ru/"; */redirectUrl;
                }

                internal User GetUserInfo(string code)
                {
                        User user = new User();

                        DiscordOAuth.Configure(_clientID, _clientSecret);

                        var scopes = new ScopesBuilder(OAuthScope.Identify);
                        var oAuth = new DiscordOAuth(_redirectUrl, scopes);

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
