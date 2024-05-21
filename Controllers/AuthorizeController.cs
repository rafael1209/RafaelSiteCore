using Microsoft.AspNetCore.Mvc;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.Model.Authorize;
using RafaelSiteCore.Model.User;
using RafaelSiteCore.Services.Auth;

namespace RafaelSiteCore.Controllers
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

                        _discordAuthLogic.ReturnUserData(user);

                        var userJsonData = new
                        {
                                Name = user.Name,
                                Balance = user.Balance,
                                AvatarUrl = $"https://cdn.discordapp.com/avatars/{user.DiscordId}/{user.AvatarHash}.png",
                                AuthToken = user.Id.ToString(),
                        };
                        return Json(userJsonData);
                }
        }
}
