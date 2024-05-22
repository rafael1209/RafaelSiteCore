using RafaelSiteCore.Model.Users;
using x3rt.DiscordOAuth2;
using x3rt.DiscordOAuth2.Entities.Enums;

namespace RafaelSiteCore.Services.Auth
{
        public class DiscordApiClient
        {
                private const ulong _clientID = 1242498004070432908;

                private static string _clientSecret = "OcrEABaN00NIWtePmBfjnAS_Co4Iy7i8";

                private const string _redirectUrl = "https://rafaelchasman.ru/";

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
                                                user.AvatarHash = userDiscord.Avatar;
                                }
                        }

                        return user;
                }
        }
}
