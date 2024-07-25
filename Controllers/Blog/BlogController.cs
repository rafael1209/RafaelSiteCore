using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Middlewere;
using RafaelSiteCore.Model.Authorize;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.Blog;
using System.ComponentModel.DataAnnotations;

namespace RafaelSiteCore.Controllers.Blog
{
        [Route("api/blog/")]
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
                [AuthMiddleware]
                public IActionResult CreatePost([FromQuery] string title, [FromQuery] string text, [FromQuery] string image)
                {
                        Request.Headers.TryGetValue("AuthToken", out var token);

                        if (!ObjectId.TryParse(token, out ObjectId userId))
                                return BadRequest("Invalid AuthToken format.");

                        var user = _authLogic.IsUserExist(token!);

                        if (user == null)
                                return Unauthorized();

                        //_blogLogic.AddPost(title, text, image, user);

                        return Ok();
                }

                [HttpPut("{postId}/like")]
                public IActionResult LikePost(string postId)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
                                return BadRequest("Invalid PostId format.");

                        var user = _authLogic.IsUserExist(token!);

                        if (user == null)
                                return Unauthorized();

                        _blogLogic.LikePost(user, ObjectId.Parse(postId));

                        return Ok();
                }

                [HttpDelete("{postId}/like")]
                public IActionResult UnlikePost([FromRoute] string postId)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
                                return BadRequest("Invalid PostId format.");

                        var user = _authLogic.IsUserExist(token!);

                        if (user == null)
                                return Unauthorized();

                        _blogLogic.UnlikePost(user, ObjectId.Parse(postId));

                        return Ok();
                }
        }
}
