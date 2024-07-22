using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.Model.Authorize;
using RafaelSiteCore.Model.Users;
using RafaelSiteCore.Services.Auth;

namespace RafaelSiteCore.Controllers.Auth
{
        [Route("api/auth")]
        [ApiController]
        public class AuthorizeController : Controller
        {
                private readonly DiscordApiClient _discordApiClient;
                private readonly DiscordAuthLogic _discordAuthLogic;
                private readonly ILogger<AuthorizeController> _logger;

                public AuthorizeController(DiscordApiClient discordApiClient, DiscordAuthLogic discordAuthLogic, ILogger<AuthorizeController> logger)
                {
                        _discordApiClient = discordApiClient;
                        _discordAuthLogic = discordAuthLogic;
                        _logger = logger;
                }

                [HttpPost("discord")]
                public IActionResult DiscordLogin([FromHeader(Name = "Code")] string code)
                {
                        try
                        {
                                if (string.IsNullOrEmpty(code))
                                {
                                        _logger.LogWarning("Invalid DiscordAuthRequest: {discordAuth}", code);
                                        return BadRequest("Invalid request");
                                }

                                User user = _discordApiClient.GetUserInfo(code);

                                if (user == null || user.DiscordId == 0) 
                                {
                                        _logger.LogWarning("User info could not be retrieved with code: {Code}", code);
                                        return BadRequest("Discord Auth Error");
                                }

                                user = _discordAuthLogic.ReturnUserData(user, user.AvatarUrl);

                                var userData = new
                                {
                                        user.Name,
                                        user.Balance,
                                        AvatarUrl = $"{user.AvatarUrl}",
                                        AuthToken = user.Id.ToString(),
                                };

                                _logger.LogInformation("User successfully authenticated: {UserData}", userData);
                                return Json(userData);
                        }
                        catch (Exception ex)
                        {
                                _logger.LogError(ex, "An error occurred during Discord login");
                                return StatusCode(500, "An internal server error occurred");
                        }
                }
        }
}
