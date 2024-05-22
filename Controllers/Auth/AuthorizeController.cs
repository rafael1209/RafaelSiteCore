using Microsoft.AspNetCore.Mvc;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.Model.Authorize;
using RafaelSiteCore.Model.Users;
using RafaelSiteCore.Services.Auth;

namespace RafaelSiteCore.Controllers.Auth
{
        [Route("api/[controller]")]
        [ApiController]
        public class AuthorizeController : Controller
        {
                private DiscordApiClient _discordApiClient;

                private DiscordAuthLogic _discordAuthLogic;

                public AuthorizeController(DiscordApiClient discordApiClient, DiscordAuthLogic discordAuthLogic)
                {
                        _discordApiClient = discordApiClient;

                        _discordAuthLogic = discordAuthLogic;
                }

                [HttpPost("DiscordLogin")]
                public IActionResult DiscordLogin([FromBody] DiscordAuthRequest discordAuth)
                {
                        User user = _discordApiClient.GetUserInfo(discordAuth.Code);

                        if (user.DiscordId == 0)
                                return BadRequest("Discord Auth Error");

                        user = _discordAuthLogic.ReturnUserData(user, user.AvatarHash);

                        var userData = new
                        {
                                user.Name,
                                user.Balance,
                                AvatarUrl = $"https://cdn.discordapp.com/avatars/{user.DiscordId}/{user.AvatarHash}.png",
                                AuthToken = user.Id.ToString(),
                        };
                        return Json(userData);
                }
        }
}
