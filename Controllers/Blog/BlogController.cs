using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Model.Authorize;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.Blog;
using System.ComponentModel.DataAnnotations;

namespace RafaelSiteCore.Controllers.Blog
{
        [Route("api/blog")]
        [ApiController]
        public class BlogController : Controller
        {
                private BlogLogic _blogLogic;
                private DiscordAuthLogic _authLogic;

                public BlogController(BlogLogic blogLogic, DiscordAuthLogic discordAuthLogic)
                {
                        _blogLogic = blogLogic;

                        _authLogic = discordAuthLogic;
                }

                [HttpGet]
                public IActionResult GetAllPosts()
                {
                        return Json(_blogLogic.GetAllPosts());
                }

                [HttpPost]
                public IActionResult CreatePost([FromHeader(Name = "AuthToken")] string authToken, [FromQuery] string title, [FromQuery] string text, [FromQuery] string image)
                {
                        if (!ObjectId.TryParse(authToken, out ObjectId userId))
                                return BadRequest("Invalid AuthToken format.");

                        var user = _authLogic.IsUserExist(ObjectId.Parse(authToken));

                        if (user == null)
                                return Unauthorized();

                        _blogLogic.AddPost(title, text, image, user);

                        return Ok();
                }

                [HttpPut("blog/{postId}/like")]
                public IActionResult LikePost([FromHeader(Name = "AuthToken")] string authToken, string postId)
                {
                        if (!ObjectId.TryParse(authToken, out ObjectId userId))
                                return BadRequest("Invalid AuthToken format.");

                        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
                                return BadRequest("Invalid PostId format.");

                        var user = _authLogic.IsUserExist(ObjectId.Parse(authToken));

                        if (user == null)
                                return Unauthorized();

                        _blogLogic.LikePost(user, ObjectId.Parse(postId));

                        return Ok();
                }

                [HttpDelete("blog/{postId}/like")]
                public IActionResult UnlikePost([FromHeader(Name = "AuthToken")] string authToken, [FromRoute] string postId)
                {
                        if (!ObjectId.TryParse(authToken, out ObjectId userId))
                                return BadRequest("Invalid AuthToken format.");

                        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
                                return BadRequest("Invalid PostId format.");

                        var user = _authLogic.IsUserExist(ObjectId.Parse(authToken));

                        if (user == null)
                                return Unauthorized();

                        _blogLogic.UnlikePost(user, ObjectId.Parse(postId));

                        return Ok();
                }
        }
}
