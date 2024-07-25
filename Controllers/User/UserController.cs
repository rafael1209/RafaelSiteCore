using Microsoft.AspNetCore.Mvc;
using RafaelSiteCore.Middlewere;
using RafaelSiteCore.Services.Auth;

namespace RafaelSiteCore.Controllers.User
{
        [Route("api/v1/user/")]
        [ApiController]
        public class UserController : Controller
        {
                private DiscordAuthLogic _authLogic;

                public UserController(DiscordAuthLogic discordAuthLogic) 
                {
                        _authLogic = discordAuthLogic;
                }

                [HttpGet]
                [AuthMiddleware]
                public IActionResult GetUser()
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        var user = _authLogic.GetUser(token!);

                        var userData = new
                        {
                                user.Name,
                                user.Balance,
                                AvatarUrl = $"{user.AvatarUrl}"
                        };

                        return Ok(userData);
                }
        }
}
